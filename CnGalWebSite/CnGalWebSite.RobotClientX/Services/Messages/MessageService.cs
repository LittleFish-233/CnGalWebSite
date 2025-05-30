﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using CnGalWebSite.RobotClientX.DataRepositories;
using CnGalWebSite.RobotClientX.Services.SensitiveWords;
using CnGalWebSite.RobotClientX.Extentions;
using CnGalWebSite.RobotClientX.Services.ExternalDatas;
using System.Text.Json;
using System.Net.Http.Json;
using CnGalWebSite.RobotClientX.Models.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CnGalWebSite.Core.Services;
using CnGalWebSite.RobotClientX.Models.Robots;
using CnGalWebSite.RobotClientX.DataModels;
using Result = CnGalWebSite.RobotClientX.Models.Messages.Result;
using CnGalWebSite.RobotClientX.Services.GPT;
using UnifyBot.Message.Chain;
using UnifyBot.Message;

namespace CnGalWebSite.RobotClientX.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<RobotReply> _robotReplyRepository;
        private readonly IRepository<RobotFace> _robotFaceRepository;
        private readonly ISensitiveWordService _sensitiveWordService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessageService> _logger;
        private readonly IExternalDataService _externalDataService;
        private readonly IHttpService _httpService;
        private readonly IChatGPTService _chatGPTService;
        private readonly IQQGroupMemberCacheService _memberCacheService;

        public MessageService(IRepository<RobotReply> robotReplyRepository, IRepository<RobotFace> robotFaceRepository, IExternalDataService externalDataService, IHttpService httpService, IChatGPTService chatGPTService,
        ILogger<MessageService> logger,
        IConfiguration configuration,
            ISensitiveWordService sensitiveWordService, IQQGroupMemberCacheService memberCacheService)
        {
            _robotReplyRepository = robotReplyRepository;
            _sensitiveWordService = sensitiveWordService;
            _logger = logger;
            _configuration = configuration;
            _robotFaceRepository = robotFaceRepository;
            _externalDataService = externalDataService;
            _httpService = httpService;
            _chatGPTService = chatGPTService;
            _memberCacheService = memberCacheService;
        }

        /// <summary>
        /// 获取可能的回复
        /// </summary>
        /// <param name="message"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public async Task<RobotReply> GetAutoReply(string message, RobotReplyRange range)
        {

            DateTime now = DateTime.Now.ToCstTime();
            List<IGrouping<int, RobotReply>> replies = _robotReplyRepository.GetAll().Where(s => s.IsHidden == false && (s.Range == RobotReplyRange.All || s.Range == range) && now.TimeOfDay <= s.BeforeTime.TimeOfDay && now.TimeOfDay >= s.AfterTime.TimeOfDay && Regex.IsMatch(message, s.Key))
                .GroupBy(s => s.Priority)
                .OrderByDescending(s => s.Key)
                .ToList();

            if (replies.Count == 0)
            {
                return null;
            }

            int index = new Random().Next(0, replies.FirstOrDefault().Count());
            RobotReply reply = replies.FirstOrDefault().ToList()[index];

            //检查是否含有变量替换 如果有 则检查输入是否包含敏感词
            if (reply.Value.Contains('$'))
            {
                List<string> words = await _sensitiveWordService.Check(message);

                if (words.Count != 0)
                {
                    return new RobotReply
                    {
                        Key = message,
                        Value = _configuration["SensitiveReply"] ?? $"{_configuration["RobotName"]}不知道哦~"
                    };
                }
            }

            return reply;
        }

        /// <summary>
        /// 处理消息回复
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="message"></param>
        /// <param name="regex"></param>
        /// <param name="qq"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgError"></exception>
        public async Task<SendMessageModel> ProcMessageAsync(RobotReplyRange range, string reply, string message, string regex, long qq, string name, long sendto)
        {

            List<KeyValuePair<string, string>> args = new();
            try
            {
                await ProcMessageArgument(reply, message, regex, qq, name, args, sendto);
                ProcMessageReplaceInput(reply, message, regex, args);
                ProcMessageFace(reply, args);
            }
            catch (ArgError arg)
            {
                reply = arg.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取变量值失败");
                reply = "呜呜呜~";
            }


            //检测敏感词
            List<string> words = await _sensitiveWordService.Check(args.Where(s => s.Key == "sender" || (s.Key.Contains('[') && s.Key.Contains(']'))).Select(s => s.Value).ToList());

            if (words.Count != 0)
            {
                string msg = $"对{name}({qq})的消息回复中包含敏感词\n消息：{message}\n回复：{reply}\n\n参数替换列表：\n";
                foreach (KeyValuePair<string, string> item in args)
                {
                    msg += $"{item.Key} -> {item.Value}\n";

                }
                msg += $"\n触发的敏感词：\n";
                foreach (string item in words)
                {
                    msg += $"{item}\n";
                }

                _logger.LogError(msg);

                throw new ArgError(msg);
            }

            //替换参数
            foreach (KeyValuePair<string, string> item in args)
            {
                reply = reply.Replace(item.Key, item.Value);
            }


            return new SendMessageModel
            {
                SendTo = qq,
                Messages = ProcMessageToOneBot(reply),
                Range = range,
                Text = reply
            };
        }

        /// <summary>
        /// 将纯文本回复转换成可发送的消息数组
        /// </summary>
        /// <param name="vaule"></param>
        /// <returns></returns>
        public MessageChain ProcMessageToOneBot(string vaule)
        {
            if (string.IsNullOrWhiteSpace(vaule))
            {
                return null;
            }

            MessageChain messages = new();

            while (true)
            {
                var item = vaule.MidStrEx("[", "]");
                if (string.IsNullOrWhiteSpace(item))
                {
                    if (string.IsNullOrWhiteSpace(vaule) == false)
                    {
                        messages.Add(new TextMessage(vaule));
                    }
                    break;
                }
                else
                {
                    var infor = "";
                    //尝试获取命令前方的字符串
                    if (vaule.StartsWith($"[{item}]") == false)
                    {
                        infor = vaule.Split($"[{item}]").FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(infor) == false)
                        {
                            messages.Add(new TextMessage(infor));
                        }
                    }
                    //删除命令及前方字符串
                    vaule = vaule.Replace($"{infor}[{item}]", "");

                    if (item.StartsWith("image="))
                    {
                        string imageStr = item.Replace("image=", "");

                        if (string.IsNullOrWhiteSpace(imageStr) == false)
                        {
                            //修正一部分图片链接缺省协议
                            if (imageStr.Contains("http") == false)
                            {
                                imageStr = "https:" + imageStr;
                            }
                            messages.Add(new ImageMessage(url: imageStr.Replace("http://image.cngal.org/", "https://image.cngal.org/").Split('?').Last()));
                        }
                    }
                    else if (item.StartsWith("声音="))
                    {
                        string voiceStr = item.Replace("声音=", "");

                        if (string.IsNullOrWhiteSpace(voiceStr) == false)
                        {
                            messages.Add(new RecordMessage(url: voiceStr.Replace("http://res.cngal.org/", "https://res.cngal.org/")));
                        }
                    }
                    else if (item.StartsWith("@"))
                    {
                        string idStr = item.Replace("@", "");
                        if (long.TryParse(idStr, out long id))
                        {
                            messages.Add(new AtMessage(id));
                        }
                    }
                    else
                    {
                        messages.Add(new TextMessage($"[{item}]"));
                    }
                }
            }

            return messages.Count > 0 ? messages : null;
        }

        /// <summary>
        /// 执行参数替换
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="message"></param>
        /// <param name="regex"></param>
        /// <param name="args"></param>
        public void ProcMessageReplaceInput(string reply, string message, string regex, List<KeyValuePair<string, string>> args)
        {
            if (string.IsNullOrWhiteSpace(regex))
            {
                return;
            }



            List<string> splits = Regex.Split(message, regex).Where(s => string.IsNullOrWhiteSpace(s) == false).ToList();


            for (int i = 0; i < splits.Count; i++)
            {
                if (reply.Contains($"[{i + 1}]"))
                {
                    args.Add(new KeyValuePair<string, string>($"[{i + 1}]", splits[i].ToString()));
                }
            }
        }

        /// <summary>
        /// 获取参数替换列表
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="message"></param>
        /// <param name="qq"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ProcMessageArgument(string reply, string message, string regex, long qq, string name, List<KeyValuePair<string, string>> args, long sendto)
        {
            while (true)
            {
                string argument = reply.MidStrEx("$(", ")");
                if (string.IsNullOrWhiteSpace(argument))
                {
                    break;
                }

                string value = argument switch
                {
                    "time" => DateTime.Now.ToCstTime().ToString("HH:mm"),
                    "qq" => qq.ToString(),
                    "weather" => await _chatGPTService.GetReply(sendto),//await _externalDataService.GetWeather(),
                    "sender" => name,
                    "n" => "\n",
                    "r" => "\r",
                    "chatgpt" => await _chatGPTService.GetReply(sendto),
                    "introduce" => await GetArgValue(argument, Regex.Split(message, regex).LastOrDefault(s => !string.IsNullOrWhiteSpace(s)).Replace("一下", ""), qq),
                    "facelist" => "该功能暂未实装",
                    _ => await GetArgValue(argument, message, qq)
                };

                reply = reply.Replace("$(" + argument + ")", value);

                args.Add(new KeyValuePair<string, string>("$(" + argument + ")", value));
            }
        }

        /// <summary>
        /// 处理表情
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="args"></param>
        public void ProcMessageFace(string reply, List<KeyValuePair<string, string>> args)
        {
            if (string.IsNullOrWhiteSpace(reply))
            {
                return;
            }

            foreach (RobotFace item in _robotFaceRepository.GetAll().Where(s => s.IsHidden == false))
            {
                if (reply.Contains($"[{item.Key}]"))
                {
                    args.Add(new KeyValuePair<string, string>($"[{item.Key}]", item.Value));
                }
            }

            for (int i = 1; i < 4; i++)
            {
                if (reply.Contains($"[{i}]") && args.Any(s => s.Key == $"[{i}]") == false)
                {
                    args.Add(new KeyValuePair<string, string>($"[{i}]", ""));
                }
            }
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="infor"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public async Task<string> GetArgValue(string name, string infor, long qq)
        {
            return await _externalDataService.GetArgValue(name, infor, qq, new Dictionary<string, string>());
        }
    }
}

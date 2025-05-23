﻿using CnGalWebSite.RobotClientX.Models.Messages;
using CnGalWebSite.RobotClientX.Models.Robots;
using CnGalWebSite.RobotClientX.DataRepositories;
using CnGalWebSite.RobotClientX.Extentions;
using CnGalWebSite.RobotClientX.Services.Events;
using CnGalWebSite.RobotClientX.Services.Messages;
using CnGalWebSite.RobotClientX.DataModels;
using CnGalWebSite.EventBus.Services;
using UnifyBot.Model;
using UnifyBot;
using System.Reactive.Linq;
using UnifyBot.Receiver.MessageReceiver;
using UnifyBot.Receiver;
using UnifyBot.Utils;
using UnifyBot.Message.Chain;
using UnifyBot.Message;

namespace CnGalWebSite.RobotClientX.Services.QQClients
{
    public class QQClientService : IQQClientService, IDisposable
    {
        private readonly IRepository<RobotGroup> _robotGroupRepository;
        private readonly IRepository<RobotEvent> _robotEventRepository;
        private readonly IRepository<PostLog> _postLogRepository;
        private readonly IConfiguration _configuration;
        private readonly IMessageService _messageService;
        private readonly IEventService _eventService;
        private readonly IEventBusService _eventBusService;
        private readonly ILogger<QQClientService> _logger;
        private readonly IGroupMessageCacheService _groupMessageCacheService;
        private readonly IQQGroupMemberCacheService _memberCacheService;


        public Bot _unifyBot { get; set; }
        private string _miraiSession;

        System.Timers.Timer t = new(1000 * 60);
        System.Timers.Timer t2 = new(1000 * 60);

        public QQClientService(IRepository<RobotGroup> robotGroupRepository, IRepository<PostLog> postLogRepository,
            IConfiguration configuration, ILogger<QQClientService> logger, IEventService eventService, IEventBusService eventBusService,
        IMessageService messageService, IRepository<RobotEvent> robotEventRepository, IGroupMessageCacheService groupMessageCacheService, IQQGroupMemberCacheService memberCacheService)
        {
            _robotGroupRepository = robotGroupRepository;
            _configuration = configuration;
            _messageService = messageService;
            _postLogRepository = postLogRepository;
            _logger = logger;
            _eventService = eventService;
            _robotEventRepository = robotEventRepository;
            _eventBusService = eventBusService;
            _groupMessageCacheService = groupMessageCacheService;
            _memberCacheService = memberCacheService;
        }

        public void InitEventBus()
        {
            if (string.IsNullOrWhiteSpace(_configuration["EventBus_HostName"]) == false)
            {
                _eventBusService.RecieveQQMessage(async (e) =>
                {
                    await SendMessage(RobotReplyRange.Friend, e.QQ, e.Message);
                });
                _eventBusService.RecieveQQGroupMessage(async (e) =>
                {
                    await SendMessage(RobotReplyRange.Group, e.GroupId, e.Message);
                });
            }
        }

        public async Task InitUnifyBot()
        {
            Connect connect = new(_configuration["OneBotHost"], int.Parse(_configuration["OneBotWsPort"]), int.Parse(_configuration["OneBotHttpPort"]), _configuration["OneBotToken"]);//token可选参数
            _unifyBot = new(connect);
            await _unifyBot.StartAsync();

            _unifyBot.MessageReceived.OfType<MessageReceiverBase>().Subscribe(x =>
            {
                //能接收到所有消息和事件
                Console.WriteLine(x.ToJsonStr());
            });
            _unifyBot.MessageReceived.OfType<MessageReceiver>().Subscribe(x =>
            {
                //只能接收到消息（所有类型）
                Console.WriteLine(x.ToJsonStr());
            });
            _unifyBot.MessageReceived.OfType<GroupReceiver>().Subscribe(async x =>
            {
                //只能接收到群消息
                Console.WriteLine(x.ToJsonStr());
                try
                {
                    await ReplyFromGroupAsync(x);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "无法回复群聊消息");
                }
            });
            _unifyBot.MessageReceived.OfType<PrivateReceiver>().Subscribe(async x =>
            {
                //只能接收到好友消息
                Console.WriteLine(x.ToJsonStr());
                try
                {
                    await ReplyFromFriendAsync(x);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "无法回复好友消息");
                }
            });
            _unifyBot.UnknownMessageReceived.OfType<string>().Subscribe(x =>
            {
                //未知事件
                Console.WriteLine(x);
            });
        }

        public async Task Init()
        {
            // 初始化onebot
            await InitUnifyBot();

            ////初始化事件总线
            InitEventBus();


            //定时任务计时器
            t.Start(); //启动计时器
            t.Elapsed += async (s, e) =>
            {
                try
                {
                    var message = _eventService.GetCurrentTimeEvent();
                    if (string.IsNullOrWhiteSpace(message) == false)
                    {
                        var result = await _messageService.ProcMessageAsync(RobotReplyRange.Group, message, "", null, 0, null, 0);

                        if (result != null)
                        {
                            foreach (var item in _robotGroupRepository.GetAll().Where(s => s.IsHidden == false && s.ForceMatch == false))
                            {
                                result.SendTo = item.GroupId;
                                await SendMessage(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "定时任务异常");
                }

            };

            //随机任务计时器
            t2.Start(); //启动计时器
            t2.Elapsed += async (s, e) =>
            {
                try
                {
                    var message = _eventService.GetProbabilityEvents();
                    if (string.IsNullOrWhiteSpace(message) == false)
                    {
                        var result = await _messageService.ProcMessageAsync(RobotReplyRange.Group, message, "", null, 0, null, 0);

                        if (result != null)
                        {
                            foreach (var item in _robotGroupRepository.GetAll().Where(s => s.IsHidden == false && s.ForceMatch == false))
                            {
                                result.SendTo = item.GroupId;
                                await SendMessage(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "随机任务异常");
                }

            };

            _logger.LogInformation("CnGal资料站 看板娘 v3.5.0");

        }

        /// <summary>
        /// 回复好友消息
        /// </summary>
        /// <returns></returns>
        public async Task ReplyFromFriendAsync(PrivateReceiver model)
        {
            string message = ConversionMeaasge(model.Message);
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            await ReplyMessageAsync(RobotReplyRange.Friend, message, model.SenderQQ, model.SenderQQ, model.Sender.Nickname);
        }

        /// <summary>
        /// 回复群聊消息
        /// </summary>
        /// <returns></returns>
        public async Task ReplyFromGroupAsync(GroupReceiver model)
        {
            //忽略未关注的群聊
            RobotGroup group = _robotGroupRepository.GetAll().FirstOrDefault(x => x.GroupId == model.GroupQQ && x.IsHidden == false);
            if (group == null)
            {
                return;
            }
            //检查是否符合强制匹配
            string message = ConversionMeaasge(model.Message);
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            // 检查需要需要更新群员列表缓存
            if (_memberCacheService.NeedUpdate(model.GroupQQ))
            {
                _memberCacheService.UpdateGroupMembers(model.GroupQQ, model.Group.Members.Select(x => new QQGroupMemberInfo
                {
                    GroupNumber = model.GroupQQ,
                    NickName = x.Nickname,
                    QQNumber = x.QQ
                }).ToList());
            }

            // 获取QQ
            var kanban = _configuration["QQ"];
            if (!long.TryParse(kanban, out long qq))
            {
                qq = 0;
            }

            // 添加消息到缓存
            _groupMessageCacheService.AddMessage(model.GroupQQ, new GroupMessageRecord
            {
                SenderId = model.SenderQQ,
                SenderName = model.Sender.Nickname,
                Content = message.ReplaceAtTags(model.GroupQQ, _memberCacheService, qq, _configuration["RobotName"]),// 替换掉@再保存
                SendTime = DateTime.Now
            });

            if (group.ForceMatch)
            {
                string name = _configuration["RobotName"] ?? "看板娘";
                if ((name != null && message.Contains(name) == false) || message.Contains("介绍") == false)
                {
                    return;
                }
            }
            await ReplyMessageAsync(RobotReplyRange.Group, message, model.GroupQQ, model.SenderQQ, model.Sender.Nickname);
        }

        /// <summary>
        /// 转换消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string ConversionMeaasge(MessageChain e)
        {
            string message = string.Concat(e.GetTextChain());
            var at = e.FirstOrDefault(s => s.Type == UnifyBot.Model.Messages.At);
            if (at != null)
            {
                long atTarget = long.Parse(((AtMessage)at).Data.QQ);
                message += "[@" + atTarget.ToString() + "]";
            }
            return message;
        }

        /// <summary>
        /// 回复消息
        /// </summary>
        private async Task ReplyMessageAsync(RobotReplyRange range, string message, long sendto, long memberId, string memberName)
        {
            //尝试找出所有匹配的回复
            RobotReply reply = await _messageService.GetAutoReply(message, range);

            if (reply == null)
            {
                return;
            }
            SendMessageModel result = new();


            try
            {
                //处理消息
                result = await _messageService.ProcMessageAsync(range, reply.Value, message, reply.Key, memberId, memberName, sendto);
            }
            catch (ArgError ae)
            {
                if (long.TryParse(_configuration["WarningQQGroup"], out long warningQQGroup))
                {
                    //发送警告
                    await SendMessage(RobotReplyRange.Group, warningQQGroup, ae.Error);
                }
            }


            if (result == null)
            {
                return;
            }

            //添加发送记录 并不代表真实发送
            _ = _postLogRepository.Insert(new PostLog
            {
                Message = message,
                PostTime = DateTime.Now.ToCstTime(),
                QQ = memberId,
                Reply = reply.Value
            });

            //检查上限
            if (await CheckLimit(range, sendto, memberId, memberName))
            {
                return;
            }

            //发送消息
            result.SendTo = sendto;
            await SendMessage(result);


        }

        /// <summary>
        /// 检查是否超过限制
        /// </summary>
        /// <param name="sendto"></param>
        /// <param name="memberId"></param>
        /// <param name="memberName"></param>
        /// <returns>是否超过限制</returns>
        private async Task<bool> CheckLimit(RobotReplyRange range, long sendto, long memberId, string memberName)
        {
            //判断该用户是否连续10次互动 1分钟内
            int singleCount = _postLogRepository.GetAll().Count(x => (DateTime.Now.ToCstTime() - x.PostTime).TotalMinutes <= 1 && x.QQ == memberId);
            int totalCount = _postLogRepository.GetAll().Count(x => (DateTime.Now.ToCstTime() - x.PostTime).TotalMinutes <= 1);

            //读取上限次数配置
            if (!long.TryParse(_configuration["SingleLimit"], out long singleLimit))
            {
                singleLimit = 5;
            }
            if (!long.TryParse(_configuration["TotalLimit"], out long totalLimit))
            {
                totalLimit = 10;
            }

            //检查上限
            if (singleCount == singleLimit)
            {
                SendMessageModel result = await _messageService.ProcMessageAsync(range, _robotEventRepository.GetAll().FirstOrDefault(s => s.Note == "消息上限警告")?.Text ?? $"[image=https://image.cngal.org/kanbanFace/hhzywx.png][@{memberId}]如果恶意骚扰人家的话，我会请你离开哦…", null, null, memberId, memberName, sendto);
                result.SendTo = sendto;
                await SendMessage(result);
                return true;
            }

            if (totalCount == totalLimit)
            {
                SendMessageModel result = await _messageService.ProcMessageAsync(range, $"核心温度过高，正在冷却......", null, null, memberId, memberName, sendto);
                result.SendTo = sendto;
                await SendMessage(result);
                return true;
            }

            if (singleCount > singleLimit)
            {
                return true;
            }

            if (totalCount > totalLimit)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public async Task SendMessage(RobotReplyRange range, long sendto, string text)
        {
            await SendMessage(new SendMessageModel
            {
                Text = text,
                SendTo = sendto,
                Messages = _messageService.ProcMessageToOneBot(text),
                Range = range,
            });
        }

        public async Task SendMessage(SendMessageModel model)
        {

            if (model == null || model.Messages == null)
            {
                return;
            }


            if (model.Range == RobotReplyRange.Friend)
            {
                await _unifyBot.SendPrivateMessage(model.SendTo, model.Messages);

                _logger.LogInformation("向 {group} 发送\"{text}\"成功", model.SendTo, model.Text);
            }
            else
            {
                await _unifyBot.SendGroupMessage(model.SendTo, model.Messages);
                _logger.LogInformation("向 {group} 发送\"{text}\"成功", model.SendTo, model.Text);

                // 添加消息到历史记录
                var kanban = _configuration["QQ"];
                if (!long.TryParse(kanban, out long qq))
                {
                    return;
                }

                _groupMessageCacheService.AddMessage(model.SendTo, new GroupMessageRecord
                {
                    SenderId = qq,
                    SenderName = _configuration["RobotName"],
                    Content = $"【看板娘】\n{model.Text}",
                    SendTime = DateTime.Now
                });
            }

            return;
        }

        public void Dispose()
        {
            if (t != null)
            {
                t.Dispose();
                t = null;
            }
            if (t2 != null)
            {
                t2.Dispose();
                t2 = null;
            }
        }

        public string GetMiraiSession()
        {
            return _miraiSession;
        }
    }
}

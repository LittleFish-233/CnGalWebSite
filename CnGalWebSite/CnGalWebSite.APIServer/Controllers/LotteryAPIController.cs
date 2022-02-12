﻿using CnGalWebSite.APIServer.Application.Articles;
using CnGalWebSite.APIServer.Application.Comments;
using CnGalWebSite.APIServer.Application.ElasticSearches;
using CnGalWebSite.APIServer.Application.Entries;
using CnGalWebSite.APIServer.Application.ErrorCounts;
using CnGalWebSite.APIServer.Application.Favorites;
using CnGalWebSite.APIServer.Application.Files;
using CnGalWebSite.APIServer.Application.Helper;
using CnGalWebSite.APIServer.Application.Messages;
using CnGalWebSite.APIServer.Application.News;
using CnGalWebSite.APIServer.Application.Perfections;
using CnGalWebSite.APIServer.Application.Peripheries;
using CnGalWebSite.APIServer.Application.Ranks;
using CnGalWebSite.APIServer.Application.Users;
using CnGalWebSite.APIServer.DataReositories;
using CnGalWebSite.APIServer.ExamineX;
using CnGalWebSite.DataModel.Helper;
using CnGalWebSite.DataModel.Model;
using CnGalWebSite.DataModel.Models;
using CnGalWebSite.DataModel.ViewModel;
using CnGalWebSite.DataModel.ViewModel.Lotteries;
using CnGalWebSite.DataModel.ViewModel.Votes;
using Markdig;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Result = CnGalWebSite.DataModel.Model.Result;


namespace CnGalWebSite.APIServer.Controllers
{


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/lotteries/[action]")]
    public class LotteryAPIController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Entry, int> _entryRepository;
        private readonly IRepository<Examine, long> _examineRepository;
        private readonly IRepository<Tag, int> _tagRepository;
        private readonly IRepository<Article, long> _articleRepository;
        private readonly IRepository<FriendLink, int> _friendLinkRepository;
        private readonly IRepository<Carousel, int> _carouselRepository;
        private readonly IRepository<UserFile, int> _userFileRepository;
        private readonly IRepository<Disambig, int> _disambigRepository;
        private readonly IRepository<Message, long> _messageRepository;
        private readonly IRepository<Comment, long> _commentRepository;
        private readonly IRepository<ThumbsUp, long> _thumbsUpRepository;
        private readonly IRepository<BackUpArchiveDetail, long> _backUpArchiveDetailRepository;
        private readonly IRepository<UserOnlineInfor, long> _userOnlineInforRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<SignInDay, long> _signInDayRepository;
        private readonly IRepository<ErrorCount, long> _errorCountRepository;
        private readonly IRepository<FavoriteFolder, long> _favoriteFolderRepository;
        private readonly IRepository<FavoriteObject, long> _favoriteObjectRepository;
        private readonly IRepository<Rank, long> _rankRepository;
        private readonly IRepository<Periphery, long> _peripheryRepository;
        private readonly IUserService _userService;
        private readonly IAppHelper _appHelper;
        private readonly IExamineService _examineService;
        private readonly IEntryService _entryService;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private readonly IErrorCountService _errorCountService;
        private readonly IFavoriteFolderService _favoriteFolderService;
        private readonly IRankService _rankService;
        private readonly IPeripheryService _peripheryService;
        private readonly IElasticsearchBaseService<Entry> _entryElasticsearchBaseService;
        private readonly IElasticsearchBaseService<Article> _articleElasticsearchBaseService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly INewsService _newsService;
        private readonly IRepository<GameNews, long> _gameNewsRepository;
        private readonly IRepository<WeeklyNews, long> _weeklyNewsRepository;
        private readonly IRepository<WeiboUserInfor, long> _weiboUserInforRepository;
        private readonly IRepository<Vote, long> _voteRepository;
        private readonly IRepository<VoteOption, long> _voteOptionRepository;
        private readonly IRepository<VoteUser, long> _voteUserRepository;
        private readonly IRepository<Lottery, long> _lotteryRepository;
        private readonly IRepository<LotteryUser, long> _lotteryUserRepository;
        private readonly IRepository<LotteryAward, long> _lotteryAwardRepository;
        private readonly IRepository<LotteryPrize, long> _lotteryPrizeRepository;

        public LotteryAPIController(IRepository<UserOnlineInfor, long> userOnlineInforRepository, IRepository<UserFile, int> userFileRepository, IRepository<FavoriteObject, long> favoriteObjectRepository,
            IRepository<Vote, long> voteRepository, IRepository<VoteOption, long> voteOptionRepository, IRepository<VoteUser, long> voteUserRepository,
        IFileService fileService, IRepository<SignInDay, long> signInDayRepository, IRepository<ErrorCount, long> errorCountRepository, IRepository<BackUpArchiveDetail, long> backUpArchiveDetailRepository,
      IRepository<ThumbsUp, long> thumbsUpRepository, IRepository<Disambig, int> disambigRepository, IRepository<BackUpArchive, long> backUpArchiveRepository, IRankService rankService, IRepository<WeiboUserInfor, long> weiboUserInforRepository,
      IRepository<ApplicationUser, string> userRepository, IMessageService messageService, ICommentService commentService, IRepository<Comment, long> commentRepository, IElasticsearchService elasticsearchService,
      IRepository<Message, long> messageRepository, IErrorCountService errorCountService, IRepository<FavoriteFolder, long> favoriteFolderRepository, IPerfectionService perfectionService, IElasticsearchBaseService<Article> articleElasticsearchBaseService,
      UserManager<ApplicationUser> userManager, IRepository<FriendLink, int> friendLinkRepository, IRepository<Carousel, int> carouselRepositor, IEntryService entryService, IElasticsearchBaseService<Entry> entryElasticsearchBaseService,
      IArticleService articleService, IUserService userService, RoleManager<IdentityRole> roleManager, IExamineService examineService, IRepository<Rank, long> rankRepository, INewsService newsService,
      IRepository<Article, long> articleRepository, IAppHelper appHelper, IRepository<Entry, int> entryRepository, IFavoriteFolderService favoriteFolderService, IRepository<Periphery, long> peripheryRepository,
      IWebHostEnvironment webHostEnvironment, IRepository<Examine, long> examineRepository, IRepository<Tag, int> tagRepository, IPeripheryService peripheryService, IRepository<GameNews, long> gameNewsRepository,
         IRepository<WeeklyNews, long> weeklyNewsRepository, IRepository<Lottery, long> lotteryRepository, IRepository<LotteryUser, long> lotteryUserRepository, IRepository<LotteryAward, long> lotteryAwardRepository,
         IRepository<LotteryPrize, long> lotteryPrizeRepository)
        {
            _userManager = userManager;
            _entryRepository = entryRepository;
            _examineRepository = examineRepository;
            _tagRepository = tagRepository;
            _appHelper = appHelper;
            _articleRepository = articleRepository;
            _examineService = examineService;
            _roleManager = roleManager;
            _userService = userService;
            _entryService = entryService;
            _articleService = articleService;
            _friendLinkRepository = friendLinkRepository;
            _carouselRepository = carouselRepositor;
            _messageRepository = messageRepository;
            _commentRepository = commentRepository;
            _commentService = commentService;
            _messageService = messageService;
            _userRepository = userRepository;
            _userFileRepository = userFileRepository;
            _userOnlineInforRepository = userOnlineInforRepository;
            _thumbsUpRepository = thumbsUpRepository;
            _disambigRepository = disambigRepository;
            _fileService = fileService;
            _signInDayRepository = signInDayRepository;
            _errorCountService = errorCountService;
            _errorCountRepository = errorCountRepository;
            _favoriteFolderRepository = favoriteFolderRepository;
            _favoriteFolderService = favoriteFolderService;
            _backUpArchiveDetailRepository = backUpArchiveDetailRepository;
            _favoriteObjectRepository = favoriteObjectRepository;
            _rankRepository = rankRepository;
            _rankService = rankService;
            _peripheryRepository = peripheryRepository;
            _peripheryService = peripheryService;
            _entryElasticsearchBaseService = entryElasticsearchBaseService;
            _articleElasticsearchBaseService = articleElasticsearchBaseService;
            _elasticsearchService = elasticsearchService;
            _newsService = newsService;
            _gameNewsRepository = gameNewsRepository;
            _weeklyNewsRepository = weeklyNewsRepository;
            _weiboUserInforRepository = weiboUserInforRepository;
            _voteRepository = voteRepository;
            _voteOptionRepository = voteOptionRepository;
            _voteUserRepository = voteUserRepository;
            _lotteryRepository = lotteryRepository;
            _lotteryUserRepository = lotteryUserRepository;
            _lotteryAwardRepository = lotteryAwardRepository;
            _lotteryPrizeRepository = lotteryPrizeRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DrawLotteryDataModel>> GetLotteryDataAsync(long id)
        {
            var lottery = await _lotteryRepository.GetAll().AsNoTracking()
               .Include(s => s.Awards).ThenInclude(s=>s.WinningUsers)
               .Include(s => s.Users).ThenInclude(s=>s.ApplicationUser)
               .FirstOrDefaultAsync(s => s.Id == id);
            var model = new DrawLotteryDataModel
            {
                Name = lottery.Name,
                Id = lottery.Id,
                NotWinningUsers=lottery.Users.Select(s=>new LotteryUserDataModel
                {
                    Id=s.ApplicationUserId,
                    Name=s.ApplicationUser.UserName,
                    Number=s.Number
                }).ToList()
            };

            foreach (var item in lottery.Awards)
            {
                model.Awards.Add(new LotteryAwardDataModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Priority = item.Priority,
                    TotalCount = item.Count,
                    WinningUsers=model.NotWinningUsers.Where(s=>item.WinningUsers.Select(s=>s.ApplicationUserId).Contains(s.Id)).ToList()
                });
                model.NotWinningUsers.RemoveAll(s => item.WinningUsers.Select(s => s.ApplicationUserId).Contains(s.Id));
            }
            return model;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<LotteryViewModel>> GetLotteryViewAsync(long id)
        {
            var lottery = await _lotteryRepository.GetAll().AsNoTracking()
                .Include(s => s.Awards)
                .Include(s => s.Users)
                .FirstOrDefaultAsync(s => s.Id == id);


            if (lottery==null)
            {
                return NotFound("未找到该抽奖");
            }

            var model = new LotteryViewModel
            {
                Id = id,
                DisplayName = lottery.DisplayName,
                BackgroundPicture = lottery.BackgroundPicture,
                BeginTime = lottery.BeginTime,
                BriefIntroduction = lottery.BriefIntroduction,
                CanComment = lottery.CanComment ?? false,
                CommentCount = lottery.CommentCount,
                Count = lottery.Users.Count,
                SmallBackgroundPicture = lottery.SmallBackgroundPicture,
                CreateTime = lottery.CreateTime,
                EndTime = lottery.EndTime,
                IsHidden = lottery.IsHidden,
                LastEditTime = lottery.LastEditTime,
                LotteryTime = lottery.LastEditTime,
                MainPage = lottery.MainPage,
                MainPicture = lottery.MainPicture,
                Name = lottery.Name,
                ReaderCount = lottery.ReaderCount,
                Thumbnail = lottery.Thumbnail,
                Type = lottery.Type
            };

            foreach(var item in lottery.Awards)
            {
                model.Awards.Add(new LotteryAwardViewModel
                {
                    Count = item.Count,
                    Id = item.Id,
                    Integral = item.Integral,
                    Name = item.Name,
                    Priority = item.Priority,
                    Type = item.Type,
                });
            }

            return model;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserLotteryStateModel>> GetUserLotteryState(long id)
        {
            var lottery=await _lotteryRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s=>s.Id == id);

            if(lottery == null)
            {
                return NotFound("未找到该抽奖");
            }
            //获取当前用户ID
            var user = await _appHelper.GetAPICurrentUserAsync(HttpContext);
            user = await _userRepository.GetAll().AsNoTracking()
                .Include(s => s.UserAddress)
                .FirstOrDefaultAsync(s => s.Id == user.Id);
            var model = new UserLotteryStateModel();

            //当前用户中奖状态
            if (user == null)
            {
                model.State = UserLotteryState.NotLogin;
            }
            else
            {
                var award = await _lotteryUserRepository.GetAll().AsNoTracking()
                    .Include(s=>s.LotteryAward)
                    .Include(s=>s.LotteryPrize)
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
                if (award == null)
                {
                    model.State = UserLotteryState.NotInvolved;
                }
                else
                {
                    if (lottery.EndTime > DateTime.Now.ToCstTime())
                    {
                        model.State = UserLotteryState.WaitingDraw;
                    }
                    else
                    {
                        if (award.LotteryAward == null)
                        {
                            model.State = UserLotteryState.NotWin;
                        }
                        else
                        {
                            model.Award = new LotteryAwardViewModel
                            {
                                Count = award.LotteryAward.Count,
                                Id = award.LotteryAward.Id,
                                Integral = award.LotteryAward.Integral,
                                Name = award.LotteryAward.Name,
                                Priority = award.LotteryAward.Priority,
                                Type = award.LotteryAward.Type,
                            };
                            if (award.LotteryAward.Type == LotteryAwardType.RealThing)
                            {


                                if (user.UserAddress == null)
                                {
                                    model.State = UserLotteryState.WaitAddress;
                                }
                                else
                                {
                                    model.State = UserLotteryState.WaitShipments;
                                }


                                if (award.LotteryPrize != null)
                                {
                                    model.State = UserLotteryState.Shipped;
                                }
                            }
                            else
                            {
                                model.State = UserLotteryState.Win;

                            }


                        }
                    }

                }
            }

            return model;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<LotteryCardViewModel>>> GetLotteryCardsAsync()
        {
            var lotteries = await _lotteryRepository.GetAll().AsNoTracking()
                .Include(s=>s.Users)
                .ToListAsync();

            var model = new List<LotteryCardViewModel>();

            foreach (var item in lotteries)
            {
                model.Add(new LotteryCardViewModel
                {
                    BeginTime = item.BeginTime,
                    BriefIntroduction = item.BriefIntroduction,
                    Count = item.Users.Count,
                    EndTime = item.EndTime,
                    Id = item.Id,
                    MainPicture = _appHelper.GetImagePath(item.MainPicture, "app.png"),
                    Name = item.Name,
                });
            }

            return model;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreateLottery(EditLotteryModel model)
        {
            if (await _voteRepository.GetAll().AnyAsync(s => s.Name == model.Name))
            {
                return new Result { Successful = false, Error = "已存在该名称的抽奖" };
            }
            //检查选项数是否合法
            if (model.Awards.Count == 0)
            {
                return new Result { Successful = false, Error = "至少需要一个奖项" };
            }
            if(model.Awards.Any(s=>s.Count==0))
            {
                return new Result { Successful = false, Error = "奖品数量不能为零" };
            }
            foreach (var item in model.Awards)
            {
                if (item.Type == LotteryAwardType.ActivationCode && item.Prizes.Count != item.Count)
                {
                    return new Result { Successful = false, Error = "激活码数量应和奖项中填写的数量对应" };
                }
            }

            Lottery lottery = new Lottery
            {
                Name = model.Name,
                EndTime = model.EndTime,
                LastEditTime = DateTime.Now.ToCstTime(),
                SmallBackgroundPicture = model.SmallBackgroundPicture,
                BackgroundPicture = model.BackgroundPicture,
                BeginTime = model.BeginTime,
                BriefIntroduction = model.BriefIntroduction,
                DisplayName = model.DisplayName,
                CreateTime = DateTime.Now.ToCstTime(),
                LotteryTime = model.LotteryTime,
                Type = model.Type,
                MainPage = model.MainPage,
                MainPicture = model.MainPicture,
                Thumbnail = model.Thumbnail,
            };

            foreach(var item in model.Awards)
            {
                var temp = new LotteryAward
                {
                    Count = item.Count,
                    Integral = item.Integral,
                    Name = item.Name,
                    Priority = item.Priority,
                    Type = item.Type,
                };
                foreach(var infor in item.Prizes)
                {
                    temp.Prizes.Add(new LotteryPrize
                    {
                        Context = infor.Context
                    });
                }

                lottery.Awards.Add(temp);

            }

            lottery = await _lotteryRepository.InsertAsync(lottery);

            return new Result { Successful = true, Error = lottery.Id.ToString() };

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<EditLotteryModel>> EditLottery(long id)
        {
            var lottery = await _lotteryRepository.GetAll().AsNoTracking()
                .Include(s => s.Awards).ThenInclude(s=>s.Prizes)
                .FirstOrDefaultAsync(s => s.Id == id);


            if (lottery == null)
            {
                return NotFound("未找到该抽奖");
            }

            var model = new EditLotteryModel
            {
                Id = id,
                DisplayName = lottery.DisplayName,
                BackgroundPicture = lottery.BackgroundPicture,
                BeginTime = lottery.BeginTime,
                BriefIntroduction = lottery.BriefIntroduction,
                SmallBackgroundPicture = lottery.SmallBackgroundPicture,
                EndTime = lottery.EndTime,
                LotteryTime = lottery.LastEditTime,
                MainPage = lottery.MainPage,
                MainPicture = lottery.MainPicture,
                Name = lottery.Name,
                Thumbnail = lottery.Thumbnail,
                Type = lottery.Type
            };

            foreach (var item in lottery.Awards)
            {
                var temp = new EditLotteryAwardModel
                {
                    Count = item.Count,
                    Id = item.Id,
                    Integral = item.Integral,
                    Name = item.Name,
                    Priority = item.Priority,
                    Type = item.Type,
                };
                foreach(var infor in item.Prizes)
                {
                    temp.Prizes.Add(new EditLotteryPrizeModel
                    {
                        Context = infor.Context,
                        Id = infor.Id,
                    });
                }
                model.Awards.Add(temp);
            }

            return model;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> EditLottery(EditLotteryModel model)
        {
            var lottery = await _lotteryRepository.GetAll()
                .Include(s => s.Awards).ThenInclude(s=>s.Prizes)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if(lottery == null)
            {
                return new Result { Successful = false, Error = "未找到该抽奖" };
            }
            if (lottery.Name!=model.Name&& await _voteRepository.GetAll().AnyAsync(s => s.Name == model.Name))
            {
                return new Result { Successful = false, Error = "已存在该名称的抽奖" };
            }
            //检查选项数是否合法
            if (model.Awards.Count == 0)
            {
                return new Result { Successful = false, Error = "至少需要一个奖项" };
            }
            if (model.Awards.Any(s => s.Count == 0))
            {
                return new Result { Successful = false, Error = "奖品数量不能为零" };
            }
            foreach (var item in model.Awards)
            {
                if (item.Type == LotteryAwardType.ActivationCode && item.Prizes.Count != item.Count)
                {
                    return new Result { Successful = false, Error = "激活码数量应和奖项中填写的数量对应" };
                }
            }


            lottery.Name = model.Name;
            lottery.EndTime = model.EndTime;
            lottery.LastEditTime = DateTime.Now.ToCstTime();
            lottery.SmallBackgroundPicture = model.SmallBackgroundPicture;
            lottery.BackgroundPicture = model.BackgroundPicture;
            lottery.BeginTime = model.BeginTime;
            lottery.BriefIntroduction = model.BriefIntroduction;
            lottery.DisplayName = model.DisplayName;
            lottery.CreateTime = DateTime.Now.ToCstTime();
            lottery.LotteryTime = model.LotteryTime;
            lottery.Type = model.Type;
            lottery.MainPage = model.MainPage;
            lottery.MainPicture = model.MainPicture;
            lottery.Thumbnail = model.Thumbnail;


            //记录现有的Ids
            var awardIds = model.Awards.Select(s => s.Id).ToList();
            awardIds.RemoveAll(s => s == 0);

            var tempList = lottery.Awards.ToList();
            tempList.RemoveAll(s => awardIds.Contains(s.Id) == false);
            lottery.Awards = tempList;

            foreach (var item in model.Awards)
            {
                if (item.Id == 0)
                {
                    var temp = new LotteryAward
                    {
                        Count = item.Count,
                        Integral = item.Integral,
                        Name = item.Name,
                        Priority = item.Priority,
                        Type = item.Type,
                    };
                    foreach (var infor in item.Prizes)
                    {
                        temp.Prizes.Add(new LotteryPrize
                        {
                            Context = infor.Context
                        });
                    }

                    lottery.Awards.Add(temp);
                }
                else
                {
                    var temp = lottery.Awards.FirstOrDefault(s => s.Id == item.Id);
                    if(temp!=null)
                    {
                        temp.Count = item.Count;
                        temp.Integral = item.Integral;
                        temp.Name = item.Name;
                        temp.Priority = item.Priority;
                        temp.Type = item.Type;

                        var prizeIds = item.Prizes.Select(s => s.Id).ToList();
                        prizeIds.RemoveAll(s => s == 0);

                        var tempList_1 = temp.Prizes.ToList();
                        tempList_1.RemoveAll(s => prizeIds.Contains(s.Id) == false);
                        temp.Prizes = tempList_1;

                        foreach(var infor in item.Prizes)
                        {
                            if(infor.Id==0)
                            {
                                temp.Prizes.Add(new LotteryPrize
                                {
                                    Context = infor.Context
                                });
                            }
                            else
                            {
                                var infor_1 = temp.Prizes.FirstOrDefault(s => s.Id == infor.Id);
                                if(infor_1!=null)
                                {
                                    infor_1.Context = infor.Context;
                                }
                            }
                        }
                    }
                }
            }

            lottery.LastEditTime = DateTime.Now;
            await _lotteryRepository.UpdateAsync(lottery);

            return new Result { Successful = true };

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> ParticipateInLottery(long id)
        {
            DateTime time = DateTime.Now.ToCstTime();
            var lottery = await _lotteryRepository.GetAll().AsNoTracking()
                .Include(s=>s.Users)
                .FirstOrDefaultAsync(s => s.Id == id && s.EndTime > time);
            if (lottery == null)
            {
                return new Result { Successful = false, Error = "未找到该抽奖或抽奖已结束" };
            }

            //获取当前用户ID
            var user = await _appHelper.GetAPICurrentUserAsync(HttpContext);

            if (lottery.Users.Any(s=>s.ApplicationUserId==user.Id))
            {
                return new Result { Successful = false, Error = "你已经参加了这个抽奖" };
            }

            await _lotteryUserRepository.InsertAsync(new LotteryUser
            {
                ApplicationUserId = user.Id,
                LotteryId = id,
                ParticipationTime = time,
                Number = lottery.Users.Count + 1
            });

            return new Result { Successful = true };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> DrawLotteryAsync(ManualLotteryModel model)
        {
            DateTime time = DateTime.Now.ToCstTime();
            if (await _lotteryRepository.GetAll().AnyAsync(s => s.Id == model.LotteryId && s.EndTime < time) == false)
            {
                return new Result { Successful = false, Error = "未找到该抽奖或抽奖未结束" };
            }

            if (await _lotteryAwardRepository.GetAll().Include(s => s.WinningUsers).AnyAsync(s => s.Id == model.LotteryAwardId && s.LotteryId == model.LotteryId && s.Count > s.WinningUsers.Count) == false)
            {
                return new Result { Successful = false, Error = "当前奖项人数已满，或未找到该奖项，请核对抽奖Id" };
            }

            var userAward=await _lotteryUserRepository.GetAll().FirstOrDefaultAsync(s=>s.ApplicationUserId == model.UserId&&s.LotteryId==model.LotteryId);

            if(userAward==null)
            {
                return new Result { Successful = false, Error = "当前用户未参加该抽奖" };
            }

            var award = await _lotteryAwardRepository.GetAll().AsNoTracking()
                .Include(s => s.WinningUsers)
                .Include(s => s.Prizes)
                .FirstOrDefaultAsync(s => s.Id == model.LotteryAwardId && s.LotteryId == model.LotteryId && s.Count > s.WinningUsers.Count);
            if (award == null)
            {
                return new Result { Successful = false, Error = "当前奖项人数已满，或未找到该奖项，请核对抽奖Id" };
            }


            userAward.LotteryAwardId = model.LotteryAwardId;
            if(award.Type== LotteryAwardType.ActivationCode)
            {
                var prize = await _lotteryPrizeRepository.FirstOrDefaultAsync(s => s.LotteryAwardId == model.LotteryAwardId && s.LotteryUserId == null);
                prize.LotteryUserId = userAward.Id;
                await _lotteryPrizeRepository.UpdateAsync(prize);
            }

            await _lotteryUserRepository.UpdateAsync(userAward);

            return new Result { Successful = true };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> HiddenLotteryAsync(HiddenLotteryModel model)
        {
            await _lotteryRepository.GetRangeUpdateTable().Where(s => model.Ids.Contains(s.Id)).Set(s => s.IsHidden, b => model.IsHidden).ExecuteAsync();
            return new Result { Successful = true };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> EditLotteryPriorityAsync(EditLotteryPriorityViewModel model)
        {
            await _lotteryRepository.GetRangeUpdateTable().Where(s => model.Ids.Contains(s.Id)).Set(s => s.Priority, b => b.Priority + model.PlusPriority).ExecuteAsync();

            return new Result { Successful = true };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrizeViewModel>> GetUserPrizeAsync(long id)
        {
            //获取当前用户ID
            var user = await _appHelper.GetAPICurrentUserAsync(HttpContext);

            var award = await _lotteryUserRepository.GetAll().AsNoTracking()
                .Include(s => s.LotteryPrize)
                .Include(s=>s.LotteryAward)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id && s.LotteryId == id);

            if(award==null)
            {
                return NotFound("未找到该奖品");
            }

            if(award.LotteryPrize==null)
            {
                return NotFound("该用户未中奖或未发放奖品");
            }

            var model = new PrizeViewModel
            {
                Context = award.LotteryPrize.Context,
                Type = award.LotteryAward.Type
            };

            return model;
        }
    }
}
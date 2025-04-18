﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnGalWebSite.Components.Models
{
    public enum IconType
    {
        None,
        [Display(Name = "词条")]
        Entry,
        Article,
        Tag,
        Tags,
        Periphery,
        Video,
        Lottery,
        Vote,
        News,
        RecentlyPublish,
        RecentlyEdit,
        ComingSoon,
        Link,
        Notice,
        Game,
        Staff,
        Role,
        Group,
        Dub,
        Make,
        Publisher,
        [Display(Name = "参与")]
        Participate,
        [Display(Name = "特别感谢")]
        SpecialThanks,
        Infor,
        Image,
        [Display(Name = "相册")]
        PhotoAlbum,
        GameRecord,
        Audio,
        MainPage,
        Home,
        Steam,
        Export,
        Comment,
        Login,
        Create,
        SeeDetails,
        Catalogs,
        EditRecord,
        Note,
        Gift,
        User,
        Users,
        Read,
        DeterminedTime,
        IndeterminateTime,
        EditTime,
        Engine,
        QQ,
        GenderMale,
        GenderFemale,
        [Display(Name = "其他性别")]
        GenderNonBinary,
        Secret,
        [Display(Name = "身材")]
        Stature,
        Data,
        Birthday,
        Hair,
        Clothes,
        [Display(Name = "瞳")]
        Pupil,
        [Display(Name = "性格")]
        Nature,
        Identity,
        [Display(Name = "血")]
        Blood,
        HumanHeight,
        Interest,
        Age,
        Name,
        AnotherName,
        RealName,
        Youtube,
        Weibo,
        Twitter,
        Facebook,
        ThumbsUp,
        Price,
        Chart,
        Discount,
        Free,
        OutOfDate,
        Takedown,
        [Display(Name = "类型")]
        Category,
        Brand,
        Size,
        Material,
        Count,
        Update,
        [Display(Name = "装饰品")]
        Ornament,
        Duration,
        Copyright,
        Sponsor,
        State,
        Filter,
        WantToPlay,
        Playing,
        Played,
        Uninterested,
        UnPlayed,
        Random,
        Switch,
        Timeline,
        Close,
        Upload,
        Loading,
        CloudUpload,
        Collapsed,
        UnCollapsed,
        Goal,
        Booking,
        UnBooking,
        Bookings,
        Edit,
        Delete,
        Check,
        Style,
        Number1, Number2, Number3, Number4, Number5, Number6, Number7,
        Right,
        Left,
        Published,
        UnPublished,
        Bug,
        QQChannel,
        Pin,
        New,
        UnknowTime,
        Carry,
        Favorite,
        UnFavorite,
        SeeMore,
        Official,
        Demo,
        EA,
        OtherReleaseType,
        Automatic,
        Manual,
        Store,
        Hide,
        Refresh,
        Prize,
        Look,
        UnRead,
        Execute,
        Up,
        Down,
        DisableComment,
        Badge,
        Score,
        UnLink,
        Run,
        Runing,
        Stop,
        Error,
        Default,
        Other,
        Move,
        Import,
        Code,
        Folder,
        Search,
        Save,
        Key,
        Lock,
        Explore,
        Shape,
        Setting,
        Calendar,
        About,
        Point,
        Certification,
        Light,
        Dark,
        Success,
        Warning,
        SignIn,
        NewbieTask,
        DailyTask,
        Task,
        Buy,
        Logout,
        Project,
        Security,
        Music,
        Programmer,
        Writer,
        Stall,
        Flag,
        Message,
        MessageRead,
        MessageUnRead,
        Record,
        Story,
        Almanac,
        GiftOpened,
        Flash,
        Board,
        UnCode,
        Send,
        Circuit,
        Bilibili,
    }

    public static class IconTypeHelper
    {
        public static string ToIconString(this IconType type)
        {
            switch (type)
            {
                case IconType.Bilibili:
                    return "mdi-television-classic";
                case IconType.Circuit:
                    return "mdi-download-network";
                case IconType.Send:
                    return "mdi-send";
                case IconType.UnCode:
                    return "mdi-barcode-off";
                case IconType.Board:
                    return " mdi-bulletin-board";
                case IconType.Flash:
                    return " mdi-flash";
                case IconType.GiftOpened:
                    return "mdi-gift-open";
                case IconType.Almanac:
                    return "mdi-book-multiple";
                case IconType.Story:
                    return "mdi-book";
                case IconType.Record:
                    return "mdi-history";
                case IconType.MessageUnRead:
                    return "mdi-message-badge";
                case IconType.MessageRead:
                    return "mdi-message-outline";
                case IconType.Message:
                    return "mdi-bell-outline";
                case IconType.Flag:
                    return "mdi-flag";
                case IconType.Stall:
                    return "mdi-tree";
                case IconType.Writer:
                    return "mdi-bookshelf";
                case IconType.Programmer:
                    return "mdi-music-accidental-sharp";
                case IconType.Music:
                    return " mdi-music";
                case IconType.Security:
                    return "mdi-security";
                case IconType.Project:
                    return "mdi-text-box-multiple";
                case IconType.Logout:
                    return "mdi-logout";
                case IconType.Buy:
                    return "mdi-shopping";
                case IconType.Task:
                    return "mdi-clipboard-check-outline";
                case IconType.DailyTask:
                    return "mdi-calendar-today";
                case IconType.NewbieTask:
                    return "mdi-new-box";
                case IconType.SignIn:
                    return "mdi-clipboard-check";
                case IconType.Success:
                    return "mdi-check-circle";
                case IconType.Warning:
                    return "mdi-alert-circle";
                case IconType.Light:
                    return "mdi-weather-sunny";
                case IconType.Dark:
                    return "mdi-weather-night";
                case IconType.Point:
                    return "mdi-vector-point";
                case IconType.Certification:
                    return "mdi-check-decagram";
                case IconType.About:
                    return "mdi-information";
                case IconType.Setting:
                    return "mdi-cog";
                case IconType.Calendar:
                    return "mdi-calendar-month";
                case IconType.Shape:
                    return "mdi-shape";
                case IconType.Explore:
                    return "mdi-compass";
                case IconType.Lock:
                    return "mdi-lock";
                case IconType.Key:
                    return "mdi-key";
                case IconType.Save:
                    return "mdi-content-save";
                case IconType.Search:
                    return "mdi-magnify";
                case IconType.Folder:
                    return "mdi-folder";
                case IconType.Code:
                    return " mdi-barcode";
                case IconType.Import:
                    return "mdi-import";
                case IconType.Move:
                    return "mdi-file-move";
                case IconType.Default:
                    return "mdi-star";
                case IconType.Other:
                    return " mdi-star-shooting-outline";
                case IconType.Error:
                    return "mdi-close-circle";
                case IconType.Runing:
                    return "mdi-play-circle-outline";
                case IconType.Run:
                    return "mdi-play";
                case IconType.Stop:
                    return "mdi-pause";
                case IconType.UnLink:
                    return "mdi-link-off";
                case IconType.Score:
                    return "mdi-star";
                case IconType.Badge:
                    return "mdi-medal-outline";
                case IconType.DisableComment:
                    return "mdi-comment-off";
                case IconType.Down:
                    return "mdi-arrow-down";
                case IconType.Up:
                    return "mdi-arrow-up";
                case IconType.Execute:
                    return "mdi-send";
                case IconType.UnRead:
                    return "mdi-read";
                case IconType.Prize:
                    return "mdi-seal-variant";
                case IconType.Refresh:
                    return "mdi-refresh";
                case IconType.Hide:
                    return "mdi-eye-off";
                case IconType.Store:
                    return "mdi-store";
                case IconType.Automatic:
                    return "mdi-autorenew";
                case IconType.Manual:
                    return "mdi-pencil";
                case IconType.Official:
                    return "mdi-check-decagram";
                case IconType.Demo:
                    return "mdi-palette-outline";
                case IconType.EA:
                    return "mdi-party-popper";
                case IconType.OtherReleaseType:
                    return "mdi-lightbulb-question";
                case IconType.SeeMore:
                    return "mdi-plus-circle";
                case IconType.Favorite:
                    return "mdi-heart";
                case IconType.UnFavorite:
                    return "mdi-heart-outline";
                case IconType.Carry:
                    return "mdi-swap-horizontal";
                case IconType.UnknowTime:
                    return "mdi-calendar-question";
                case IconType.New:
                    return "mdi-new-box";
                case IconType.Pin:
                    return "mdi-pin";
                case IconType.QQChannel:
                    return "mdi-music-accidental-sharp";
                case IconType.Bug:
                    return "mdi-bug";
                case IconType.Published:
                    return "mdi-publish";
                case IconType.UnPublished:
                    return "mdi-publish-off";
                case IconType.Left:
                    return "mdi-chevron-left";
                case IconType.Right:
                    return "mdi-chevron-right";
                case IconType.Number1:
                    return "mdi-numeric-1-box-multiple-outline";
                case IconType.Number2:
                    return "mdi-numeric-2-box-multiple-outline";
                case IconType.Number3:
                    return "mdi-numeric-3-box-multiple-outline";
                case IconType.Number4:
                    return "mdi-numeric-4-box-multiple-outline";
                case IconType.Number5:
                    return "mdi-numeric-5-box-multiple-outline";
                case IconType.Number6:
                    return "mdi-numeric-6-box-multiple-outline";
                case IconType.Number7:
                    return "mdi-numeric-7-box-multiple-outline";
                case IconType.Style:
                    return "mdi-palette";
                case IconType.Check:
                    return "mdi-check-bold";
                case IconType.Delete:
                    return "mdi-trash-can";
                case IconType.Edit:
                    return "mdi-pencil";
                case IconType.Booking:
                    return "mdi-heart";
                case IconType.Bookings:
                    return "mdi-heart-multiple";
                case IconType.UnBooking:
                    return "mdi-heart-outline";
                case IconType.Goal:
                    return "mdi-flag-checkered";
                case IconType.Collapsed:
                    return "mdi-chevron-right";
                case IconType.UnCollapsed:
                    return "mdi-chevron-down";
                case IconType.CloudUpload:
                    return "mdi-cloud-upload";
                case IconType.Loading:
                    return "mdi-loading";
                case IconType.Upload:
                    return "mdi-upload";
                case IconType.Close:
                    return "mdi-close";
                case IconType.Timeline:
                    return "mdi-timeline-text-outline ";
                case IconType.Switch:
                    return "mdi-swap-horizontal";
                case IconType.Random:
                    return "mdi-cube-send ";
                case IconType.WantToPlay:
                    return "mdi-book ";
                case IconType.Playing:
                    return "mdi-google-controller";
                case IconType.Played:
                    return "mdi-play-box-multiple";
                case IconType.Uninterested:
                    return "mdi-thumb-down";
                case IconType.UnPlayed:
                    return "mdi-pin-off";
                case IconType.Filter:
                    return "mdi-filter ";
                case IconType.State:
                    return "mdi-sign-real-estate ";
                case IconType.Sponsor:
                    return "mdi-bullhorn-variant-outline";
                case IconType.Duration:
                    return "mdi-timer-outline";
                case IconType.Copyright:
                    return "mdi-copyright ";
                case IconType.Ornament:
                    return "mdi-ornament ";
                case IconType.Update:
                    return "mdi-update";
                case IconType.Count:
                    return "mdi-counter";
                case IconType.Material:
                    return "mdi-palette-swatch ";
                case IconType.Brand:
                    return "mdi-tag-heart ";
                case IconType.Size:
                    return "mdi-move-resize";
                case IconType.Category:
                    return "mdi-shape ";
                case IconType.Takedown:
                    return "mdi-download-off";
                case IconType.OutOfDate:
                    return "mdi-database-off ";
                case IconType.Free:
                    return "mdi-cloud";
                case IconType.Discount:
                    return "mdi-brightness-percent";
                case IconType.Chart:
                    return "mdi-chart-bar";
                case IconType.Price:
                    return "mdi-cash";
                case IconType.ThumbsUp:
                    return "mdi-thumb-up ";
                case IconType.Facebook:
                    return "mdi-facebook";
                case IconType.Twitter:
                    return "mdi-twitter ";
                case IconType.Youtube:
                    return "mdi-youtube ";
                case IconType.Weibo:
                    return "mdi-sina-weibo ";
                case IconType.RealName:
                    return "mdi-card-account-details-star ";
                case IconType.Name:
                    return "mdi-card-account-details ";
                case IconType.AnotherName:
                    return "mdi-card-account-details-outline ";
                case IconType.Age:
                    return "mdi-forest ";
                case IconType.Interest:
                    return "mdi-heart-multiple ";
                case IconType.HumanHeight:
                    return "mdi-human-male-height-variant ";
                case IconType.Blood:
                    return "mdi-water ";
                case IconType.Identity:
                    return "mdi-account-outline ";
                case IconType.Nature:
                    return "mdi-nature-people ";
                case IconType.Pupil:
                    return "mdi-eye ";
                case IconType.Look:
                    return "mdi-eye ";
                case IconType.Clothes:
                    return "mdi-hanger ";
                case IconType.Hair:
                    return "mdi-face-man-profile ";
                case IconType.Birthday:
                    return "mdi-cake-variant ";
                case IconType.Stature:
                    return "mdi-human-handsup ";
                case IconType.Data:
                    return "mdi-database ";
                case IconType.Secret:
                    return "mdi-lock";
                case IconType.GenderNonBinary:
                    return "mdi-gender-non-binary ";
                case IconType.GenderMale:
                    return "mdi-gender-male ";
                case IconType.GenderFemale:
                    return "mdi-gender-female ";
                case IconType.QQ:
                    return "mdi-qqchat";
                case IconType.Engine:
                    return "mdi-engine-outline ";
                case IconType.Entry:
                    return "mdi-archive";
                case IconType.Article:
                    return "mdi-script-text";
                case IconType.Tag:
                    return "mdi-tag";
                case IconType.Tags:
                    return "mdi-tag-multiple ";
                case IconType.Video:
                    return "mdi-animation-play ";
                case IconType.Periphery:
                    return "mdi-basket";
                case IconType.Lottery:
                    return "mdi-wallet-giftcard";
                case IconType.Vote:
                    return "mdi-vote";
                case IconType.News:
                    return "mdi-newspaper-variant-outline ";
                case IconType.RecentlyPublish:
                    return "mdi-publish ";
                case IconType.RecentlyEdit:
                    return "mdi-history ";
                case IconType.ComingSoon:
                    return "mdi-av-timer ";
                case IconType.Link:
                    return "mdi-link";
                case IconType.Notice:
                    return "mdi-bullhorn-variant-outline ";
                case IconType.Game:
                    return "mdi-gamepad-square ";
                case IconType.Staff:
                    return "mdi-sitemap";
                case IconType.Role:
                    return "mdi-account-supervisor ";
                case IconType.Group:
                    return "mdi-group ";
                case IconType.Dub:
                    return "mdi-microphone ";
                case IconType.Make:
                    return "mdi-auto-fix";
                case IconType.Publisher:
                    return "mdi-send  ";
                case IconType.Participate:
                    return "mdi-sitemap ";
                case IconType.SpecialThanks:
                    return "mdi-flower-outline ";
                case IconType.Infor:
                    return "mdi-information-outline  ";
                case IconType.Image:
                    return "mdi-image";
                case IconType.GameRecord:
                    return "mdi-message-draw ";
                case IconType.PhotoAlbum:
                    return "mdi-folder-multiple-image ";
                case IconType.Audio:
                    return "mdi-volume-high";
                case IconType.MainPage:
                    return "mdi-text-box-outline ";
                case IconType.Home:
                    return "mdi-home ";
                case IconType.Steam:
                    return "mdi-steam ";
                case IconType.Export:
                    return "mdi-database-export";
                case IconType.Comment:
                    return "mdi-comment-text";
                case IconType.Login:
                    return "mdi-login";
                case IconType.Create:
                    return "mdi-plus";
                case IconType.SeeDetails:
                    return "mdi-share-all-outline";
                case IconType.Catalogs:
                    return "mdi-format-list-bulleted-square ";
                case IconType.EditRecord:
                    return "mdi-file-document-edit-outline  ";
                case IconType.Note:
                    return "mdi-note-outline ";
                case IconType.Gift:
                    return "mdi-gift ";
                case IconType.User:
                    return "mdi-account ";
                case IconType.Users:
                    return "mdi-account-multiple  ";
                case IconType.Read:
                    return "mdi-eye";
                case IconType.DeterminedTime:
                    return "mdi-clock-outline ";
                case IconType.EditTime:
                    return "mdi-clock-edit-outline ";
                case IconType.IndeterminateTime:
                    return "mdi-clock";
                default:
                    return "";
            }

        }
    }

}

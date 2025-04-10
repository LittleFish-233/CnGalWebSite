﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnGalWebSite.Shared.Service
{
    public interface IEventService
    {
        event Action SavaTheme;
        event Action CleanTempEffectTheme;
        event Action<bool?, bool?,bool?, string> TempEffectTheme;
        event Action ToggleMiniMode;
        event Action<string> ChangeTitle;
        event Action SwitchEntryStyle;
        event Action KanbanChanged;
        event Action UserInfoChanged;
        event Action UserCommodityChanged;
        event Action ThemeChanged;

        void OnKanbanChanged();

        void OnSavaTheme();

        void OnCleanTempEffectTheme();

        Task OpenNewPage(string url);

        void OnTempEffectTheme(bool? isDark, bool? isFullScreen, bool? isTransparent, string themeColor);

        void OnToggleMiniMode();

        void OnChangeTitle(string title);

        void OnSwitchStyle();

        void OnUserInfoChanged();

        void OnUserCommodityChanged();

        void OnThemeChanged();
    }
}

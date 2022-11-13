// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Properties;
using CatalanViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Catalan;
using CommonViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Common;
using CroatianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Croatian;
using CzechViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Czech;
using DanishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Danish;
using DutchViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Dutch;
using EnglishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.English;
using FinnishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Finnish;
using FrenchViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.French;
using GeorgianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Georgian;
using GermanViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.German;
using GreekViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Greek;
using HebrewViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Hebrew;
using HindiViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Hindi;
using ItalianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Italian;
using HungarianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Hungarian;
using JapaneseViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Japanese;
using KoreanViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Korean;
using PersianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Persian;
using PolishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Polish;
using PortugueseViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Portuguese;
using RussianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Russian;
using SerbianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Serbian;
using SlovakViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Slovak;
using SlovenianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Slovenian;
using SpanishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Spanish;
using TurkishViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Turkish;
using UkrainianViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Ukrainian;
using UrduViews = JuliusSweetland.OptiKey.UI.Views.Keyboards.Urdu;
using ViewModelKeyboards = JuliusSweetland.OptiKey.UI.ViewModels.Keyboards;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base
{
    public abstract class Keyboard : IKeyboard
    {
        protected Keyboard(bool simulateKeyStrokes = true, bool multiKeySelectionSupported = false)
        {
            SimulateKeyStrokes = simulateKeyStrokes;
            MultiKeySelectionSupported = multiKeySelectionSupported;
        }

        public bool SimulateKeyStrokes { get; private set; }
        public bool MultiKeySelectionSupported { get; private set; }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public object GetContent()
        {
            var Keyboard = this;
            object newContent = null;
            if (Keyboard is ViewModelKeyboards.Alpha1)
            {
                if (Settings.Default.UsingCommuniKateKeyboardLayout)
                {
                    newContent = (object)new CommonViews.CommuniKate { DataContext = Keyboard };
                }
                else switch (Settings.Default.KeyboardAndDictionaryLanguage)
                    {
                        case Languages.CatalanSpain:
                            newContent = new CatalanViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.CroatianCroatia:
                            newContent = new CroatianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.CzechCzechRepublic:
                            newContent = new CzechViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.DanishDenmark:
                            newContent = new DanishViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.DutchBelgium:
                            newContent = new DutchViews.BelgiumAlpha { DataContext = Keyboard };
                            break;
                        case Languages.DutchNetherlands:
                            newContent = new DutchViews.NetherlandsAlpha { DataContext = Keyboard };
                            break;
                        case Languages.FinnishFinland:
                            newContent = new FinnishViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.FrenchCanada:
                            newContent = new FrenchViews.CanadaAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.FrenchFrance:
                            newContent = new FrenchViews.FranceAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GeorgianGeorgia:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new GeorgianViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : new GeorgianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GermanGermany:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new GermanViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : Settings.Default.UseAlphabeticalKeyboardLayout
                                    ? (object)new GermanViews.AlphabeticalAlpha1 { DataContext = Keyboard }
                                    : new GermanViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GreekGreece:
                            newContent = new GreekViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HebrewIsrael:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new HebrewViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : new HebrewViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HindiIndia:
                            newContent = new HindiViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HungarianHungary:
                            newContent = new HungarianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.ItalianItaly:
                            newContent = Settings.Default.UseAlphabeticalKeyboardLayout
                                ? (object)new ItalianViews.AlphabeticalAlpha1 { DataContext = Keyboard }
                                : new ItalianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.JapaneseJapan:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new JapaneseViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : new JapaneseViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.KoreanKorea:
                            newContent = new KoreanViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PersianIran:
                            newContent = new PersianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PolishPoland:
                            newContent = new PolishViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PortuguesePortugal:
                            newContent = new PortugueseViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.RussianRussia:
                            newContent = new RussianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SerbianSerbia:
                            newContent = new SerbianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SlovakSlovakia:
                            newContent = new SlovakViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SlovenianSlovenia:
                            newContent = new SlovenianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SpanishSpain:
                            newContent = new SpanishViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.TurkishTurkey:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new TurkishViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : new TurkishViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.UkrainianUkraine:
                            newContent = new UkrainianViews.Alpha1 { DataContext = Keyboard };
                            break;
                        case Languages.UrduPakistan:
                            newContent = new UrduViews.Alpha1 { DataContext = Keyboard };
                            break;
                        default:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new EnglishViews.SimplifiedAlpha1 { DataContext = Keyboard }
                                : Settings.Default.UseAlphabeticalKeyboardLayout
                                    ? (object)new EnglishViews.AlphabeticalAlpha1 { DataContext = Keyboard }
                                    : new EnglishViews.Alpha1 { DataContext = Keyboard };
                            break;
                    }
            }
            else if (Keyboard is ViewModelKeyboards.Alpha2)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HebrewIsrael:
                        newContent = new HebrewViews.Alpha2 { DataContext = Keyboard };
                        break;
                    case Languages.HindiIndia:
                        newContent = new HindiViews.Alpha2 { DataContext = Keyboard };
                        break;
                    case Languages.JapaneseJapan:
                        newContent = Settings.Default.UseSimplifiedKeyboardLayout
                            ? (object)new JapaneseViews.SimplifiedAlpha2 { DataContext = Keyboard }
                            : new JapaneseViews.Alpha2 { DataContext = Keyboard };
                        break;
                    case Languages.KoreanKorea:
                        newContent = new KoreanViews.Alpha2 { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.Alpha2 { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.Alpha2 { DataContext = Keyboard };
                        break;
                }
            }
            //else if (Keyboard is ViewModelKeyboards.Alpha3)
            //{
            //    switch (Settings.Default.KeyboardAndDictionaryLanguage)
            //    {
            //        case Languages.PlaceholderForALanguageWith3AlphaKeyboards:
            //            newContent = new PlaceholderForALanguageWith3AlphaKeyboardsViews.Alpha3 {DataContext = Keyboard};
            //            break;
            //    }
            //}
            else if (Keyboard is ViewModelKeyboards.ConversationAlpha1)
            {
                if (Settings.Default.UsingCommuniKateKeyboardLayout)
                {
                    newContent = (object)new CommonViews.CommuniKate { DataContext = Keyboard };
                }
                else switch (Settings.Default.KeyboardAndDictionaryLanguage)
                    {
                        case Languages.CatalanSpain:
                            newContent = new CatalanViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.CroatianCroatia:
                            newContent = new CroatianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.CzechCzechRepublic:
                            newContent = new CzechViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.DanishDenmark:
                            newContent = new DanishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.DutchBelgium:
                            newContent = new DutchViews.BelgiumConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.DutchNetherlands:
                            newContent = new DutchViews.NetherlandsConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.FinnishFinland:
                            newContent = new FinnishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.FrenchCanada:
                            newContent = new FrenchViews.CanadaConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.FrenchFrance:
                            newContent = new FrenchViews.FranceConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GeorgianGeorgia:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new GeorgianViews.SimplifiedConversationAlpha1 { DataContext = Keyboard }
                                : new GeorgianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GermanGermany:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new GermanViews.SimplifiedConversationAlpha1 { DataContext = Keyboard }
                                : Settings.Default.UseAlphabeticalKeyboardLayout
                                    ? (object)new GermanViews.AlphabeticalConversationAlpha1 { DataContext = Keyboard }
                                    : new GermanViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.GreekGreece:
                            newContent = new GreekViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HebrewIsrael:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new HebrewViews.SimplifiedConversationAlpha1 { DataContext = Keyboard }
                                : new HebrewViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HindiIndia:
                            newContent = new HindiViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.HungarianHungary:
                            newContent = new HungarianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.ItalianItaly:
                            newContent = Settings.Default.UseAlphabeticalKeyboardLayout
                                ? (object)new ItalianViews.AlphabeticalConversationAlpha1 { DataContext = Keyboard }
                                : new ItalianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.JapaneseJapan:
                            newContent = new JapaneseViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.KoreanKorea:
                            newContent = new KoreanViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PersianIran:
                            newContent = new PersianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PolishPoland:
                            newContent = new PolishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.PortuguesePortugal:
                            newContent = new PortugueseViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.RussianRussia:
                            newContent = new RussianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SerbianSerbia:
                            newContent = new SerbianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SlovakSlovakia:
                            newContent = new SlovakViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SlovenianSlovenia:
                            newContent = new SlovenianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.SpanishSpain:
                            newContent = new SpanishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.TurkishTurkey:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new TurkishViews.SimplifiedConversationAlpha1 { DataContext = Keyboard }
                                : new TurkishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.UkrainianUkraine:
                            newContent = new UkrainianViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        case Languages.UrduPakistan:
                            newContent = new UrduViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                        default:
                            newContent = Settings.Default.UseSimplifiedKeyboardLayout
                                ? (object)new EnglishViews.SimplifiedConversationAlpha1 { DataContext = Keyboard }
                                : Settings.Default.UseAlphabeticalKeyboardLayout
                                    ? (object)new EnglishViews.AlphabeticalConversationAlpha1 { DataContext = Keyboard }
                                    : new EnglishViews.ConversationAlpha1 { DataContext = Keyboard };
                            break;
                    }
            }
            else if (Keyboard is ViewModelKeyboards.ConversationAlpha2)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HindiIndia:
                        newContent = new HindiViews.ConversationAlpha2 { DataContext = Keyboard };
                        break;
                    case Languages.JapaneseJapan:
                        newContent = new JapaneseViews.ConversationAlpha2 { DataContext = Keyboard };
                        break;
                    case Languages.KoreanKorea:
                        newContent = new KoreanViews.ConversationAlpha2 { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.ConversationAlpha2 { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.ConversationAlpha2 { DataContext = Keyboard };
                        break;
                }
            }
            //else if (Keyboard is ViewModelKeyboards.ConversationAlpha3)
            //{
            //    switch (Settings.Default.KeyboardAndDictionaryLanguage)
            //    {
            //        case Languages.PlaceholderForALanguageWith3AlphaKeyboards:
            //            newContent = new PlaceholderForALanguageWith3AlphaKeyboardsViews.ConversationAlpha3 { DataContext = Keyboard };
            //            break;
            //    }
            //}
            else if (Keyboard is ViewModelKeyboards.ConversationConfirm)
            {
                newContent = new CommonViews.ConversationConfirm { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.ConversationNumericAndSymbols)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HindiIndia:
                        newContent = new HindiViews.ConversationNumericAndSymbols { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.ConversationNumericAndSymbols { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.ConversationNumericAndSymbols { DataContext = Keyboard };
                        break;
                    default:
                        newContent = new CommonViews.ConversationNumericAndSymbols { DataContext = Keyboard };
                        break;
                }
            }
            else if (Keyboard is ViewModelKeyboards.Currencies1)
            {
                newContent = new CommonViews.Currencies1 { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Currencies2)
            {
                newContent = new CommonViews.Currencies2 { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Diacritics1)
            {
                newContent = new CommonViews.Diacritics1 { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Diacritics2)
            {
                newContent = new CommonViews.Diacritics2 { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Diacritics3)
            {
                newContent = new CommonViews.Diacritics3 { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Language)
            {
                newContent = new CommonViews.Language { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Voice)
            {
                var voice = Keyboard as ViewModelKeyboards.Voice;

                // Since the Voice keyboard's view-model is in charge of creating the keys instead of the
                // view, we need to make sure any localized text is up-to-date with the current UI language.
                voice.LocalizeKeys();

                newContent = new CommonViews.Voice { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Menu)
            {
                newContent = new CommonViews.Menu { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Minimised)
            {
                newContent = new CommonViews.Minimised { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.Mouse)
            {
                newContent = new CommonViews.Mouse { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.NumericAndSymbols1)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HindiIndia:
                        newContent = new HindiViews.NumericAndSymbols1 { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.NumericAndSymbols1 { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.NumericAndSymbols1 { DataContext = Keyboard };
                        break;
                    default:
                        newContent = new CommonViews.NumericAndSymbols1 { DataContext = Keyboard };
                        break;
                }
            }
            else if (Keyboard is ViewModelKeyboards.NumericAndSymbols2)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HindiIndia:
                        newContent = new HindiViews.NumericAndSymbols2 { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.NumericAndSymbols2 { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.NumericAndSymbols2 { DataContext = Keyboard };
                        break;
                    default:
                        newContent = new CommonViews.NumericAndSymbols2 { DataContext = Keyboard };
                        break;
                }
            }
            else if (Keyboard is ViewModelKeyboards.NumericAndSymbols3)
            {
                switch (Settings.Default.KeyboardAndDictionaryLanguage)
                {
                    case Languages.HindiIndia:
                        newContent = new HindiViews.NumericAndSymbols3 { DataContext = Keyboard };
                        break;
                    case Languages.PersianIran:
                        newContent = new PersianViews.NumericAndSymbols3 { DataContext = Keyboard };
                        break;
                    case Languages.UrduPakistan:
                        newContent = new UrduViews.NumericAndSymbols3 { DataContext = Keyboard };
                        break;
                    default:
                        newContent = new CommonViews.NumericAndSymbols3 { DataContext = Keyboard };
                        break;
                }
            }
            else if (Keyboard is ViewModelKeyboards.PhysicalKeys)
            {
                newContent = new CommonViews.PhysicalKeys { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.SizeAndPosition)
            {
                newContent = new CommonViews.SizeAndPosition { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.WebBrowsing)
            {
                newContent = new CommonViews.WebBrowsing { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.YesNoQuestion)
            {
                newContent = new CommonViews.YesNoQuestion { DataContext = Keyboard };
            }
            else if (Keyboard is ViewModelKeyboards.DynamicKeyboardSelector)
            {
                var kb = Keyboard as ViewModelKeyboards.DynamicKeyboardSelector;
                newContent = new CommonViews.DynamicKeyboardSelector(kb.PageIndex) { DataContext = Keyboard };
            }

            return newContent;
        }
    }
}

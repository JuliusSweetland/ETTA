// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

namespace JuliusSweetland.OptiKey.Models
{
    public interface ILayout
    {
        double ScreenWidth { get; }
        double ScreenHeight { get; }
        double ScreenLeft { get; }
        double ScreenTop { get; }
        double Left { get; }
        double Top { get; }
        double Width { get; }
        double Height { get; }
        int Rows { get; }
        int Columns { get; }
    }
}

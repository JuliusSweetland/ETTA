// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
namespace JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base
{
    public interface IKeyboard
    {
        bool SimulateKeyStrokes { get; }
        bool MultiKeySelectionSupported { get; }
        void OnEnter();
        void OnExit();
        object GetContent();
    }
}

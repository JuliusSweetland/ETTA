// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JuliusSweetland.OptiKey.Models
{
    public class InteractorProfileMap : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public InteractorProfileMap(InteractorProfile profile, bool isMember)
        {
            Profile = profile;
            IsMember = isMember;
        }

        public InteractorProfile Profile { get; private set; }

        private bool isMember;
        public bool IsMember
        {
            get { return isMember; }
            set { isMember = Profile.Name == "All" || value; OnPropertyChanged(); }
        }
    }
}
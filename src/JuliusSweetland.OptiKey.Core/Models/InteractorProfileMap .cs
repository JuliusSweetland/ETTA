// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JuliusSweetland.OptiKey.Models
{
    public class InteractorProfileMap : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler descendantPropertyChanged;
        public ObservableCollection<InteractorProfile> Descendants { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (isMember && !Descendants.Contains(Profile))
                Descendants.Add(Profile);
            else if (!isMember && Descendants.Contains(Profile))
                Descendants.Remove(Profile);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void DescendantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public InteractorProfileMap(InteractorProfile profile, bool isMember)
        {
            descendantPropertyChanged = new PropertyChangedEventHandler(DescendantPropertyChanged);

            Descendants = new ObservableCollection<InteractorProfile>();
            Descendants.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (INotifyPropertyChanged propertyChanged in e.NewItems)
                        propertyChanged.PropertyChanged += descendantPropertyChanged;
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (INotifyPropertyChanged propertyChanged in e.OldItems)
                        propertyChanged.PropertyChanged -= descendantPropertyChanged;
                }
            };

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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TweetSharp;
using MinimalMVVM.ViewModels;

namespace MinimalMVVM
{
    public class WebBrowserHelper
    {
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.RegisterAttached("Body", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(OnBodyChanged));

        public static string GetBody(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BodyProperty);
        }

        public static void SetBody(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(BodyProperty, body);
        }

        private static void OnBodyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = (WebBrowser)d;
            webBrowser.Loaded += delegate
            {
                webBrowser.Navigate((string)e.NewValue);
            };
        }
    }

    public class TwitterBrowserViewModel : ObservableObject, IRequireViewIdentification
    {
        TwitterUserModel twitterUserModel;

        public TwitterBrowserViewModel()
        {
            twitterUserModel = new TwitterUserModel();
            _viewId = Guid.NewGuid();
            CreateTwitterClient();
        }

        public ICommand SendCodeCommand
        {
            get { return new DelegateCommand(SendCode, true); }
        }

        public void SendCode()
        {
            CheckCode();
        }

        private Guid _viewId;
        public Guid ViewID
        {
            get { return _viewId; }
        }
        
        public Uri PinUrl
        {
            get
            {
                return twitterUserModel.PinUrl;
            }
            set
            {
                twitterUserModel.PinUrl = value;
                RaisePropertyChangedEvent(nameof(PinUrl));
            }
        }

        public string Verifier
        {
            get
            {
                return twitterUserModel.Verifier;
            }
            set
            {
                twitterUserModel.Verifier = value;
                RaisePropertyChangedEvent(nameof(Verifier));
            }
        }

        public void CreateTwitterClient()
        {
            twitterUserModel.CreateTwitterService();
        }

        public void CheckCode()
        {
            if (twitterUserModel.TryLogin() == true)
                WindowManagerModel.CloseWindow(ViewID);
        }
    }
}

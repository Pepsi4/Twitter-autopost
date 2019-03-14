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

        public TwitterBrowserViewModel()
        {
            _viewId = Guid.NewGuid();
          CreateTwitterClient();
        }

        private Uri htmlDoc;
        public Uri HtmlDoc
        {
            get
            {
                
                return htmlDoc;
            }
            set
            {
                htmlDoc = value;
            }
        }
        private string verifier;
        public string Verifier
        {
            get
            {
                return verifier;
            }
            set
            {
                verifier = value;
                RaisePropertyChangedEvent(nameof(Verifier));
            }
        }


        OAuthRequestToken token;
        TwitterService service;

        public void CreateTwitterClient()
        {
            TwitterClientInfo client = new TwitterClientInfo();
            client.ConsumerKey = "BBfi7kpS5sR2Ad5FrkrhuVe7y";
            client.ConsumerSecret = "BX5pLTGiQ4sqQXIHdEaQKL44wxmyes9E8KtyE7j08hmxhW1p0t";
            service = new TwitterService(client);
            token = service.GetRequestToken();
            HtmlDoc = service.GetAuthorizationUri(token);
        }

        public void CheckCode()
        {
            MessageBox.Show("test");
            OAuthAccessToken accessToken =
                service.GetAccessToken(token, Verifier);


            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            WindowManagerModel.CloseWindow(ViewID);
        }
    }
}

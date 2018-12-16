using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TweetSharp;
using System.Windows.Input;
using MinimalMVVM.ViewModels;

namespace MinimalMVVM
{
    class UsersProfileViewModel : ObservableObject
    {
        private string _usersName;
        public string UsersName
        {
            get { return _usersName; }
            set { _usersName = value; }
        }

        private static string ConsumerKey = "v9bAQmu0Rw1v03hGc8WmafGqm";
        private static string ConsumerSecret = "nGwlUdqOePtqtNeFJVYfsagqhzqRvqjeK81Qqm3butPtNG8Rar";
        private static string _accessToken = "936666867258753024-ZCvQoYYlu1bbbYPzP44NyTBWRIltPxE";
        private static string _accessTokenSecret = "DLLbB105wfh3EJMHt9dxSXZIu5zhlQso4NtNDw9Szhslj";

        private static TwitterService service = new TwitterService(ConsumerKey, ConsumerSecret, _accessToken, _accessTokenSecret);
        private List<string> _allUsersList = new List<string>();

        //private ICommand _updateAllUsersListCommand;
        public ICommand UpdateAllUsersListCommand
        {
            get
            {
                return new DelegateCommand(Test, true);
            }
        }

        private void Test()
        {
            TwitterUser tuSelf = service.GetUserProfile(
    new GetUserProfileOptions() { IncludeEntities = false, SkipStatus = false });

            ListFollowersOptions options = new ListFollowersOptions();
            options.UserId = tuSelf.Id;
            options.ScreenName = tuSelf.ScreenName;
            options.IncludeUserEntities = true;
            options.SkipStatus = false;
            options.Cursor = -1;
            List<TwitterUser> lstFollowers = new List<TwitterUser>();

            TwitterCursorList<TwitterUser> followers = service.ListFollowers(options);

            // if the API call did not succeed
            if (followers == null)
            {
                // handle the error
                // see service.Response and/or service.Response.Error for details
            }
            else
            {
                while (followers.NextCursor != null)
                {
                    //options.Cursor = followers.NextCursor;
                    //followers = m_twService.ListFollowers(options);

                    // if the API call did not succeed
                    if (followers == null)
                    {
                        // handle the error
                        // see service.Response and/or service.Response.Error for details
                    }
                    else
                    {
                        foreach (TwitterUser user in followers)
                        {
                            // do something with the user (I'm adding them to a List)
                            lstFollowers.Add(user);
                        }
                    }

                    // if there are more followers
                    if (followers.NextCursor != null &&
                        followers.NextCursor != 0)
                    {
                        // then advance the cursor and load the next page of results
                        options.Cursor = followers.NextCursor;
                        followers = service.ListFollowers(options);
                    }
                    // otherwise, we're done!
                    else
                        break;
                }
            }

            System.Windows.MessageBox.Show(followers[0].Name);
        }

        //private void UpdateAllUsersList()
        //{
        //    var profile = service.GetUserProfileFor(new GetUserProfileForOptions
        //    {
        //        ScreenName = _usersName
        //        , Co
        //    });

        //    for (int i = -1; i < profile.FollowersCount; i++)
        //    {
        //        var users = service.ListFollowers(new ListFollowersOptions
        //        {
        //            ScreenName = _usersName,
                   
        //        });

        //        _allUsersList.Add(users[0].Name);
        //    }




        //    //foreach (var item in users)
        //    //{
        //    //    _allUsersList.Add(item.Name);
        //    //}

        //    FileModel fileModel = new FileModel();
        //    fileModel.SaveChangesInFile(FileModel.AllUsersFilePath, _allUsersList);

        //   // System.Windows.MessageBox.Show(users.Count.ToString());

        //    System.Windows.MessageBox.Show(_allUsersList[0]);
        //    System.Windows.MessageBox.Show(_allUsersList[1]);
        //    System.Windows.MessageBox.Show(_allUsersList.Count.ToString());
        //}
    }
}

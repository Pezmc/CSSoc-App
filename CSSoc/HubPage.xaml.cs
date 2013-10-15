using CSSoc.Common;
using CSSoc.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Facebook;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=??????

namespace CSSoc
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    /// 

    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        FacebookClient _fb = new FacebookClient();
        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            IEnumerable<FacebookEvent> events = await FacebookDataSource.GetEventsAsync();
        
            int upcomingEventCount = (events.Count()-1);

            IEnumerable<FacebookEventGroup> group = await FacebookDataSource.GetGroupsAsync();

            var groupSelect = group.Take(1);

            this.DefaultViewModel["UpcomingEvents"] = groupSelect.Take(1);

            // The last item in the list if the first event
            var testObj = events.Skip(upcomingEventCount).Take(1);
            var test = testObj.ElementAt(0);
            this.DefaultViewModel["NextEvent"] = test;
            /*dynamic res = await _fb.GetTaskAsync("oauth/access_token", new
            {
                client_id = "378133422317406",
                client_secret = "b8f6ebf52f9fd41cb4e19583f6cce00b",
                grant_type = "client_credentials"
            });
            _fb.AccessToken = res.access_token;

            dynamic parameters = new ExpandoObject();
            parameters.fields = "events.fields(cover,description,location,attending.fields(id),maybe.fields(id),start_time,end_time,name,is_date_only),albums.limit(10).fields(name,photos.limit(60).fields(source))";
            dynamic result = await _fb.GetTaskAsync("cssoc.man", parameters);

            Facebook.JsonArray events = result.events.data;

            //dynamic nextEvent = events[0];
            this.DefaultViewModel["NextEvent"] = new SampleDataItem("main", nextEvent.name, nextEvent.start_time, nextEvent.cover.source, nextEvent.description, "Content");*/
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        /*void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var fbevent = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((FacebookEvent)fbevent).Id);
        }*/

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((FacebookEvent)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}

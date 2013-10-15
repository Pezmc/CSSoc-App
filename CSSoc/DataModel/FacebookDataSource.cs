using Facebook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace CSSoc.Data
{

    public class FacebookEventGroup
    {

        public FacebookEventGroup(string id)
        {

            this.uId = id;
            this.Items = new ObservableCollection<FacebookEvent>();

        }

        public string uId { get; private set; }

        public ObservableCollection<FacebookEvent> Items { get; private set; }
    }

    public class FacebookPage
    {
        public string Id { get; private set; }
        public ObservableCollection<FacebookEvent> Events { get; private set; }

        public FacebookPage(dynamic pageInfo)
        {
            this.Id = pageInfo.id;

            this.Events = new ObservableCollection<FacebookEvent>();
            foreach(dynamic eventInfo in pageInfo.events.data) {
                this.Events.Add(new FacebookEvent(eventInfo));
            }
            
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class FacebookEvent
    {
        public FacebookEvent(dynamic eventInfo)
        {
            this.Id = eventInfo.id;
            this.AttendingCount = eventInfo.attending.data.Count;
            this.CoverImage = eventInfo.cover.source;
            this.Description = eventInfo.description;
            //if (!String.IsNullOrEmpty(eventInfo.end_time))
                //this.EndTime = DateTime.Parse(eventInfo.end_time);
            this.StartTime = DateTime.Parse(eventInfo.start_time);
            this.IsDateOnly = eventInfo.is_date_only;
            this.MaybeCount = eventInfo.maybe.data.Count;
            this.Location = eventInfo.location;
            this.Name = eventInfo.name;
            // cover,description,location,attending.fields(id),maybe.fields(id),start_time,end_time,name,is_date_only
        }

        public string Id { get; private set; }
        public string Location { get; private set; }
        public string Name { get; private set; }
        public int AttendingCount { get; private set; }
        public int MaybeCount { get; private set; }
        public string CoverImage { get; private set; }    
        public string Description { get; private set; }
        public DateTime EndTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public Boolean IsDateOnly { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class FacebookDataSource
    {
        private static FacebookDataSource _dataSource = null;

        private ObservableCollection<FacebookEventGroup> _group = new ObservableCollection<FacebookEventGroup>();

        public ObservableCollection<FacebookEventGroup> Group
        {
            get { return this._group; }
        }

        static FacebookDataSource()
        {
            _dataSource = new FacebookDataSource();
        }

        public static async Task<IEnumerable<FacebookEvent>> GetEventsAsync()
        {
            await _dataSource.GetFacebookDataAsync();

            return _dataSource.Events;
        }

        public static async Task<IEnumerable<FacebookEventGroup>> GetGroupsAsync()
        {
            await _dataSource.GetFacebookDataAsync();

            return _dataSource.Group;
        }


        private FacebookPage _page = null;
        public ObservableCollection<FacebookEvent> Events
        {
            get {
                if (this._page == null) return null;

                return this._page.Events;
            }
        }

        public static async Task<FacebookEvent> GetEventAsync(string id)
        {
            await _dataSource.GetFacebookDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _dataSource.Events.Where((e) => e.Id.Equals(id));
            return matches.First();
        }

        /*public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }*/

        FacebookClient _fb = new FacebookClient();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task GetFacebookDataAsync()
        {
            if (this.Events != null && this.Events.Count != 0)
                return;

            dynamic facebookReply = await _fb.GetTaskAsync("oauth/access_token", new
            {
                client_id = "378133422317406",
                client_secret = "b8f6ebf52f9fd41cb4e19583f6cce00b",
                grant_type = "client_credentials"
            });
            _fb.AccessToken = facebookReply.access_token;

            dynamic parameters = new ExpandoObject();
            parameters.fields = "events.fields(cover,description,location,attending.fields(id),maybe.fields(id),start_time,end_time,name,is_date_only),albums.limit(10).fields(name,photos.limit(60).fields(source))";
            dynamic cssocManData = await _fb.GetTaskAsync("cssoc.man", parameters);

            this._page = new FacebookPage(cssocManData);

            FacebookEventGroup group = new FacebookEventGroup("1");
            foreach (dynamic eventInfo in cssocManData.events.data)
            {
                group.Items.Add(new FacebookEvent(eventInfo));
            }
            group.Items.Remove(group.Items.Last());
            this.Group.Add(group);
        }
    }
}
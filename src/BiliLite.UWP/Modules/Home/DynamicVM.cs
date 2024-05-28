using BiliLite.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BiliLite.Modules
{
    public class DynamicVM : IModules
    {
        readonly Api.User.DynamicAPI dynamicAPI;
        public DynamicVM()
        {
            dynamicAPI = new Api.User.DynamicAPI();
            dynamicItemDataTemplateSelector = new DynamicItemDataTemplateSelector();
            RefreshCommand = new RelayCommand(Refresh);
            LoadMoreCommand = new RelayCommand(LoadMore);
        }
        public DynamicItemDataTemplateSelector dynamicItemDataTemplateSelector { get; set; }
        private bool _loading = true;
        public bool Loading
        {
            get { return _loading; }
            set { _loading = value; DoPropertyChanged("Loading"); }
        }
        public ICommand RefreshCommand { get; private set; }
        public ICommand LoadMoreCommand { get; private set; }
        private ObservableCollection<DynamicItemModel> _Items;

        public ObservableCollection<DynamicItemModel> Items
        {
            get { return _Items; }
            set { _Items = value; DoPropertyChanged("Items"); }
        }

        public async Task GetDynamicItems(string idx = "")
        {
            try
            {
                Loading = true;
                var api = dynamicAPI.DyanmicNew(Api.User.DynamicAPI.UserDynamicType.Video);
                if (idx != "")
                {
                    api = dynamicAPI.HistoryDynamic(idx, Api.User.DynamicAPI.UserDynamicType.Video);
                }
                var results = await api.Request();
                if (results.status)
                {
                    var data = results.GetJObject();
                    if (data["code"].ToInt32() == 0)
                    {
                        var items = JsonConvert.DeserializeObject<ObservableCollection<DynamicItemModel>>(data["data"]["items"].ToString());
                        if (Items == null)
                        {
                            Items = items;
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                Items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        Utils.ShowMessageToast(data["message"].ToString());
                    }
                }
                else
                {
                    Utils.ShowMessageToast(results.message);

                }
            }
            catch (Exception ex)
            {
                var handel = HandelError<AnimeHomeModel>(ex);
                Utils.ShowMessageToast(handel.message);
            }
            finally
            {
                Loading = false;
            }
        }
        public async void Refresh()
        {
            if (Loading)
            {
                return;
            }
            Items = null;
            await GetDynamicItems();
        }
        public async void LoadMore()
        {
            if (Loading)
            {
                return;
            }
            if (Items == null || Items.Count == 0)
            {
                return;
            }
            var last = Items.LastOrDefault();
            await GetDynamicItems(last.desc.dynamic_id);
        }



      
    }
    public class DynamicItemModel
    {
        /// <summary>
        /// json字符串
        /// </summary>
        public string extend_json { get; set; }
        /// <summary>
        /// json字符串,根据desc里的type，获得数据
        /// </summary>
        public string card { get; set; }
        public DynamicDescModel desc { get; set; }
        public DynamicCardModel2024 video
        {
            get
            {
                if (desc != null && desc.type == 8)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DynamicCardModel2024>(card);
                }
                return null;
            }
        }
        public DynamicSeasonCardModel season
        {
            get
            {
                if (desc != null && desc.type == 512)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DynamicSeasonCardModel>(card);
                }
                return null;
            }
        }
    }
    public class DynamicItemDataTemplateSelector : DataTemplateSelector
    {
        public ResourceDictionary resource;
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var card = item as DynamicItemModel;
            if (card.desc.type == 8)
            {
                return resource["DynamicVideo"] as DataTemplate;
            }
            else
            {
                return resource["DynamicSeason"] as DataTemplate;
            }
        }
    }
    public class DynamicDescModel
    {
        public string uid { get; set; }
        /// <summary>
        /// 8=视频，512=番剧
        /// </summary>
        public int type { get; set; }
        public string rid { get; set; }
        public int view { get; set; }
        public int like { get; set; }
        public int comment { get; set; }
        public int is_liked { get; set; }
        public string dynamic_id_str { get; set; }
        public string dynamic_id { get; set; }
        public int status { get; set; }
        public long timestamp { get; set; }
    }
    public class DynamicVideoCardModel
    {
        public string aid { get; set; }
        public int attribute { get; set; }
        public string cid { get; set; }
        public long ctime { get; set; }
        public string desc { get; set; }
        public int duration { get; set; }
        public string dynamic { get; set; }
        public string jump_url { get; set; }
        public DynamicVideoCardOwnerModel owner { get; set; }
        public string pic { get; set; }
        public long pubdate { get; set; }
        public string title { get; set; }
        public DynamicVideoCardStatModel stat { get; set; }
    }
    public class DynamicVideoCardStatModel
    {
        public int coin { get; set; }
        public int danmaku { get; set; }
        public int favorite { get; set; }
        public int like { get; set; }
        public int reply { get; set; }
        public int share { get; set; }
        public int view { get; set; }
    }
    public class DynamicVideoCardOwnerModel
    {
        public string face { get; set; }
        public string mid { get; set; }
        public string name { get; set; }
    }
    public class DynamicSeasonCardModel
    {
        public string aid { get; set; }
        public string cover { get; set; }
        public string index_title { get; set; }
        public string index { get; set; }
        public string new_desc { get; set; }
        public string url { get; set; }
        public int play_count { get; set; }
        public int reply_count { get; set; }
        public int bullet_count { get; set; }
        public int episode_id { get; set; }
        public DynamicSeasonCardApiSeasonInfoModel season { get; set; }
    }
    public class DynamicSeasonCardApiSeasonInfoModel
    {
        public string type_name { get; set; }
        public string cover { get; set; }
        public string title { get; set; }
        public int is_finish { get; set; }
        public int season_id { get; set; }
    }
    public class DynamicCardModel2024
    {
        public DynamicCardModel2024_basic basic { get; set; }
        public DynamicCardModel2024_modules modules { get; set; }
        public DynamicCardModel2024 orig { get; set; }
        public string type { get; set; }
        public string id_str { get; set; }
        public bool visible { get; set; }
        public string title
        {
            get { return 
                    modules?.module_dynamic?.major?.archive?.title
                    ??
                    modules?.module_dynamic?.major?.live?.title
                    ??
                    modules?.module_dynamic?.major?.article?.title
                    ??
                    modules?.module_dynamic?.major?.ugc_season?.title
                    ??
                    modules?.module_dynamic?.major?.pgc?.title
                    ??
                    "";
            }
        }
        public string jumpurl
        {
            get
            {
                var url=
                    modules?.module_dynamic?.major?.archive?.jump_url ??
                    modules?.module_dynamic?.major?.article?.jump_url ??
                    modules?.module_dynamic?.major?.pgc?.jump_url ??
                    modules?.module_dynamic?.major?.ugc_season?.jump_url ??
                    modules?.module_dynamic?.major?.live?.jump_url ??
                     "";
                if (url.Length==0) return url;
                if (url[0] == '/') return "https:" + url;
                return url;
            }
        }
        public string durationtext
        {
            get
            {
                return
                    modules.module_dynamic.major?.archive?.duration_text ??
                    modules.module_dynamic.major?.ugc_season?.duration_text ?? "";
            }
        }

        public long pubdate
        {
            get { return modules?.module_author?.pub_ts??0; }
        }
        public string face
        {
            get { return modules?.module_author?.face??""; }
        }

        public string owner
        {
            get { return modules?.module_author?.name??""; }
        }

        public long mid
        {
            get
            {
                return modules?.module_author?.mid??0;
            }
        }

        public string pic
        {
            get { return 
                    modules?.module_dynamic?.major?.archive?.cover??
                    modules?.module_dynamic?.major?.live?.cover??
                    modules?.module_dynamic?.major?.article?.covers?[0]??
                    modules?.module_dynamic?.major?.pgc?.cover??
                    modules?.module_dynamic?.major?.ugc_season?.cover??
                    "";
            }
        }
        public string aid
        {
            get { return 
                    modules?.module_dynamic?.major?.archive?.aid??
                    modules?.module_dynamic?.major?.live?.id?.ToString()??
                    basic.rid_str??
                    "0"; }
        }
        public string danmakutext
        {
            get { return modules?.module_dynamic?.major?.archive?.stat?.danmaku??"0"; }
        }
        public int danmaku
        {
            get
            {
                var text = danmakutext;
                text = text.Replace("万", "0000").Replace("千", "000").Replace("百", "00").Replace("十", "0");
                int result;
                if (Int32.TryParse(text, out result))
                {
                    return result;
                }
                return 0;
            }
        }
        public int view
        {
            get {
                var text = viewtext;
                text = text.Replace("万", "0000").Replace("千","000").Replace("百","00").Replace("十","0");
                int result;
                if (Int32.TryParse(text, out result))
                {
                    return result;
                }
                return 0; }
        }
        public string viewtext
        {
            get
            {
                return modules?.module_dynamic?.major?.archive?.stat?.play ??
                  modules.module_dynamic.major?.article?.label ??
                  modules.module_dynamic.major?.live?.desc_second ??
                  modules.module_dynamic.major?.pgc?.stat?.play ??
                  modules.module_dynamic.major?.ugc_season?.stat?.play ??
                  "0";
            }
        }
        public int like
        {
            get {
                return  modules?.module_stat?.like?.count??0;
            }
        }
        public int comment
        {
            get {
                return modules?.module_stat?.comment?.count??0;
            }
        }
        public int forward
        {
            get {
                return modules?.module_stat?.forward?.count??0;
            }
        }
        public bool liked
        {
            get
            {
                return modules?.module_stat?.like?.status??false;
            }
        }
        public string desc
        {
            get
            {
                return
                    modules?.module_dynamic?.desc?.text ??
                    modules?.module_dynamic?.major?.article?.desc ??
                    "";
            }
        }
    }
    public class DynamicCardModel2024_basic {
        public string comment_id_str { get; set; }
        public int comment_type { get; set; }
        public string rid_str { get; set; }
    }
    public class DynamicCardModel2024_modules
    {
        public DynamicCardModel2024_author module_author;
        public DynamicCardModel2024_stat module_stat;
        public DynamicCardModel2024_dynamic module_dynamic;
    }
    public class DynamicCardModel2024_author
    {
        public DynamicCardModel2024_avatar avatar { get; set; }
        public DynamicCardModel2024_decorate decorate { get; set; }
        public DynamicCardModel2024_vip vip{ get; set; }
        public DynamicCardModel2024_off official_verify{ get; set; }
        public DynamicCardModel2024_pendant pendant{ get; set; }
        public long mid { get; set; }
        public string name { get; set; }
        public string pub_time { get; set; }
        public string type { get; set; }
        public string pub_action { get; set; }
        public string pub_location_text { get; set; }
        public string label { get; set; }
        public string face { get; set; }
        public long pub_ts { get; set; }
    }
    public class DynamicCardModel2024_avatar
    {
        public string mid { get; set; }
    }
    public class DynamicCardModel2024_off
    {
        public string desc { get; set; }
        public int type { get; set; }
    }
    public class DynamicCardModel2024_vip
    {
        public string nickname_color { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public int theme_type { get; set; }
    }
    public class DynamicCardModel2024_pendant
    {
        public string image { get; set; }
        public string image_enhance { get; set; }
        public int expire { get; set; }
        public int pid { get; set; }
        public int n_pid { get; set; }
        public string name { get; set; }
    }
    public class DynamicCardModel2024_decorate
    {
        public string name { get; set; }
        public string card_url { get; set; }
        public string jump_url { get; set; }
        public DynamicCardModel2024_fan fan { get; set; }
        public int type { get; set; }
    }
    public class DynamicCardModel2024_face
    {
        public string mid { get; set; }
    }
    public class DynamicCardModel2024_fan
    {
        public bool is_fan { get; set; }
        public string color { get; set; }
        public int number { get; set; }
    }
    public class DynamicCardModel2024_stat
    {
        public DynamicCardModel2024_like like { get; set; }
        public DynamicCardModel2024_forward forward { get; set; }
        public DynamicCardModel2024_comment comment { get; set; }
    }
    public class DynamicCardModel2024_like
    {
        public int count { get; set; }
        public bool forbidden { get; set; }
        public bool status { get; set; }
    }
    public class DynamicCardModel2024_forward
    {
        public int count { get; set; }
        public bool forbidden { get; set; }
    }
    public class DynamicCardModel2024_comment
    {
        public int count { get; set; }
        public bool forbidden { get; set; }
    }
    public class DynamicCardModel2024_dynamic
    {
        //public string additional { get; set; }
        public DynamicCardModel2024_desc desc { get; set; }
        //public string topic { get; set; }
        public DynamicCardModel2024_major major {get; set; }
    }
    public class DynamicCardModel2024_desc
    {
        public System.Collections.Generic.List<DynamicCardModel2024_node> rich_text_nodes { get; set; }
        public string text { get; set; }
    }
    public class DynamicCardModel2024_node
    {
        public string orig_text { get; set; }
        public string text { get; set; }
        public string type { get; set; }
}
    public class DynamicCardModel2024_major
    {
        public DynamicCardModel2024_archive archive { get; set; }
        public DynamicCardModel2024_draw draw { get; set; }
        public DynamicCardModel2024_live live { get; set; }
        public DynamicCardModel2024_article article { get; set; }
        public DynamicCardModel2024_season pgc{ get; set; }
        public DynamicCardModel2024_list ugc_season{ get; set; }
        public DynamicCardModel2024_live_rcmd live_rcmd { get; set; }
        public string type { get; set; }
    }
    public class DynamicCardModel2024_live_rcmd
    {
        public string content { get; set; }
    }
    public class DynamicCardModel2024_article
    {
        public System.Collections.Generic.List<string> covers { get; set; }
        public string desc { get; set; }
        public int id { get; set; }
        public string jump_url{ get; set; }
        public string label{ get; set; }
        public string title{ get; set; }
    }
    public class DynamicCardModel2024_season
    {
        //public object badge{ get; set; }
        public string cover{ get; set; }
        public int epid{ get; set; }
        public int season_id{ get; set; }
        public DynamicCardModel2024_astat stat { get; set; }
        /*1：番剧
2：电影
3：纪录片
4：国创
5：电视剧
6：漫画
7：综艺*/
        public int sub_type{ get; set; }
        public string jump_url{ get; set; }
        public int type{ get; set; }
        public string title{ get; set; }
    }
    public class DynamicCardModel2024_list
    {
        public long aid{ get; set; }
        //public object badge{ get; set; }
        public string cover{ get; set; }
        public string desc{ get; set; }
        public int disable_preview{ get; set; }
        public string duration_text{ get; set; }
        public string jump_url{ get; set; }
        public DynamicCardModel2024_astat stat { get; set; }
        public string title{ get; set; }
    }
    public class DynamicCardModel2024_live
    {
        //public obj badge {get; set;}
        public string cover { get; set; }
        public string desc_first{ get; set; }
        public string desc_second{ get; set; }
        public string id{ get; set; }
        public string jump_url{ get; set; }
        public int live_state{ get; set; }
        public string title{ get; set; }
        public int style { get; set; }

    }
    public class DynamicCardModel2024_archive
    {
        public string aid { get; set; }
        public DynamicCardModel2024_badge badge { get; set; }
        public string bvid { get; set; }
        public string cover { get; set; }
        public string desc { get; set; }
        public int disable_preview { get; set; }
        public string duration_text { get; set; }
        public string jump_url { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public DynamicCardModel2024_astat stat;
    }
    public class DynamicCardModel2024_badge
    {
        public string bg_color { get; set; }
        public string color { get; set; }
        public string text { get; set; }
        //public string icon_url { get; set; }
    }
    public class DynamicCardModel2024_draw
    {
        public long id { get; set; }
        public System.Collections.Generic.List<DynamicCardModel2024_pic> items { get; set; }
    }
    public class DynamicCardModel2024_pic
    {
        public int height { get; set; }
        public int width { get; set; }
        public float size { get; set; }
        public string src{ get; set; }
    }
    public class DynamicCardModel2024_astat
    {
        public string danmaku { get; set; }
        public string play { get; set; }
    }
}

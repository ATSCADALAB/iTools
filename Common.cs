using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ATSCADA.iWinTools
{
    [ServiceContract]
    public interface InterfaceWebPort
    {
        [OperationContract]
        UInt64 GetClientCounter();

        [OperationContract]
        void TagListClear();

        [OperationContract]
        void AddTag(WebTag Tag);

        [OperationContract]
        WebTag GetTag(string Tagname);//Format: Task.Tag

        [OperationContract]
        void UpdateTag(string Tagname, string TagValue, string TagStatus, string TagTimeStamp);//Format: Task.Tag

        [OperationContract]
        void WriteTagValue(string TagName, string ValuetoWrite);

    }

    [DataContract]
    public class WebTag
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string TimeStamp { get; set; }
        [DataMember]
        public string Value { get; set; } = "";
        [DataMember]
        public string ValuetoWrite { get; set; } = "";
    }

    //WCF Service for iWebPort
    [ServiceBehavior(UseSynchronizationContext = false)]
    public class iWebPortServer : InterfaceWebPort, IDisposable
    {
        protected static double _ClientCounter = -1;

        protected static List<WebTag> TagList = new List<WebTag>();

        public UInt64 GetClientCounter()
        {
            if (_ClientCounter >= 0)
                return (UInt64)_ClientCounter;
            else
                return 0;
        }

        public void TagListClear()
        {
            //TagList.Clear();
        }

        public void AddTag(WebTag Tag)
        {
            if (Tag is null) return;
            Tag.Name = Tag.Name.DecryptAddress();
            Tag.Value = Tag.Value.DecryptValue();
            TagList.Add(Tag);
        }

        public WebTag GetTag(string Tagname)
        {
            var nameDecrypt = Tagname.DecryptAddress();
            var webtag = TagList.Find(delegate (WebTag _t) { return _t.Name == nameDecrypt; });
            if (webtag is null) return null;
            return new WebTag()
            {
                Name = Tagname,
                Value = webtag.Value.EncryptValue(),
                ValuetoWrite = webtag.ValuetoWrite.EncryptValue(),
                Status = webtag.Status,
                TimeStamp = webtag.TimeStamp
            };
        }

        public void UpdateTag(string Tagname, string TagValue, string TagStatus, string TagTimeStamp)
        {
            var nameDecrypt = Tagname.DecryptAddress();

            WebTag _wt = TagList.Find(delegate (WebTag _t) { return _t.Name == nameDecrypt; });
            if (_wt != null)
            {
                _wt.Value = TagValue.DecryptValue();
                _wt.Status = TagStatus;
                _wt.TimeStamp = TagTimeStamp;
            }
        }

        public void WriteTagValue(string Tagname, string ValuetoWrite)
        {
            var nameDecrypt = Tagname.DecryptAddress();

            WebTag _wt = TagList.Find(delegate (WebTag _t) { return _t.Name == nameDecrypt; });
            if (_wt != null)
                _wt.ValuetoWrite = ValuetoWrite.DecryptValue();
        }

        public iWebPortServer()
        {
            _ClientCounter++;
        }
        public void Dispose()
        {
            _ClientCounter--;
        }
    }

}

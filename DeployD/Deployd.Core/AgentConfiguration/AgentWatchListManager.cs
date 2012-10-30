using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Raven.Client;

namespace Deployd.Core.AgentConfiguration
{
    public interface IAgentWatchListManager
    {
        IAgentWatchList Load();
        void SaveWatchList(IAgentWatchList agentWatchList);
        void SaveWatchList(string agentWatchList);
    }

    public interface IWatchListStore
    {
        IAgentWatchList Load();
        void Save(IAgentWatchList watchList);
    }

    public class DocumentWatchListStore : IWatchListStore
    {
        private readonly IDocumentSession _session;

        public DocumentWatchListStore(IDocumentSession session)
        {
            _session = session;
        }

        public IAgentWatchList Load()
        {
            return _session.Load<AgentWatchList>(1);
        }

        public void Save(IAgentWatchList watchList)
        {
            var existing = Load();
            if (existing == null)
            {
                _session.Store(watchList);
            } else
            {
                existing.Groups = watchList.Groups;
                existing.Packages = watchList.Packages;
            }
        }
    }

    public class AgentWatchListManager : IAgentWatchListManager
    {
        private readonly IWatchListStore _watchListStore;
        private static object _fileLock=new object();
        private IAgentWatchList _watchList;

        public AgentWatchListManager(IWatchListStore watchListStore)
        {
            _watchListStore = watchListStore;
        }

        public IAgentWatchList Load()
        {
            if (_watchList == null)
            {
                _watchList = _watchListStore.Load() ?? new AgentWatchList();
            }
            return _watchList;
        }

        public void SaveWatchList(IAgentWatchList agentWatchList)
        {
            _watchListStore.Save(agentWatchList);
        }

        public IAgentWatchList LoadWatchList()
        {
            lock (_fileLock)
            {
                using (var fs = new FileStream("~\\watchList.config".MapVirtualPath(), FileMode.Open))
                {
                    return (IAgentWatchList)new XmlSerializer(typeof(AgentWatchList)).Deserialize(fs);
                }
            }
        }

        public void SaveWatchList(string agentWatchList)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(new StringReader(agentWatchList), settings))
            {
                var serialized = new XmlSerializer(typeof (AgentWatchList)).Deserialize(reader);
            }
            lock (_fileLock)
            {

                using (var fs = new FileStream("~\\watchList.config".MapVirtualPath(), FileMode.Create))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(agentWatchList);
                    fs.Flush();
                }
            }
        }
    }
}
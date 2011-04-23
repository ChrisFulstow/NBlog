using System.Collections.Generic;
using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service
{
    public interface IEntryService
    {
        void Save(Entry entry);
        Entry GetBySlug(string slug);
        List<Entry> GetList();
        void Delete(string slug);
        bool Exists(string slug);
    }
}
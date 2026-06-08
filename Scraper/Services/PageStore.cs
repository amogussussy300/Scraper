using Scraper.Core.Dtos;
namespace Scraper.Services;
public class PageStore
{
    private readonly Dictionary<int, List<PageDto>> _bySource = new();
    private readonly Dictionary<int, (int sourceId, PageDto page)> _byPageId = new();
    private readonly object _lock = new();
    private int _nextId = 1;
    public (int pageId, PageDto page) Create(int sourceId, string name, string url)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("URL is required");
        lock (_lock)
        {
            var page = new PageDto { Name = name, Url = url };
            if (!_bySource.TryGetValue(sourceId, out var list))
                _bySource[sourceId] = list = new();
            list.Add(page);
            var id = _nextId++;
            _byPageId[id] = (sourceId, page);
            return (id, page);
        }
    }
    public PageDto? Get(int pageId)
    {
        lock (_lock)
            return _byPageId.TryGetValue(pageId, out var t) ? t.page : null;
    }
    public IReadOnlyList<(int pageId, PageDto page)> GetWithIdsForSource(int sourceId)
    {
        lock (_lock)
            return _byPageId
                .Where(kv => kv.Value.sourceId == sourceId)
                .Select(kv => (kv.Key, kv.Value.page))
                .ToList();
    }
    public void Update(int pageId, PageDto replacement)
    {
        lock (_lock)
        {
            if (!_byPageId.TryGetValue(pageId, out var t)) return;
            var list = _bySource[t.sourceId];
            var idx = list.IndexOf(t.page);
            if (idx >= 0) list[idx] = replacement;
            _byPageId[pageId] = (t.sourceId, replacement);
        }
    }
    public void Delete(int pageId)
    {
        lock (_lock)
        {
            if (!_byPageId.TryGetValue(pageId, out var t)) return;
            _bySource[t.sourceId].Remove(t.page);
            _byPageId.Remove(pageId);
        }
    }
}
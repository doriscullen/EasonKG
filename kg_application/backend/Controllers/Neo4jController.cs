using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
using Neo4j.Driver;
using System.Text;

[Route("api/")] 
[ApiController] 
public class Neo4jController : ControllerBase 
{ 
    private readonly IDriver _driver;

    public Neo4jController() 
    { 
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "ZJUKG2022"));   
    }

    [HttpGet] 
    public async Task<IActionResult> Search(string query) 
    { 
        if (query[0] == '专' && query[1]=='辑')
        {
            var album = query.Remove(0, 2);
            album = album.Remove(album.Length-7, 7);
            string s = String.Format("查询该专辑中的歌曲：{0}", album);
            var res = await  _getAlbumSongs(album);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        
        if (query[0] == '歌' && query[1]=='曲')
        {
            var song = query.Remove(0, 2);
            song = song.Remove(song.Length-7, 7);
            string s = String.Format("查询该歌曲的发行日期：{0}", song);
            var res = await  _getDate(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        
        else if (query.Length>7 && query[^7] == '演' && query[^6]=='唱')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询该歌手所唱的歌曲：{0}", person);
            var res = await  _getSingerSongs(person);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^7] == '作' && query[^6]=='词')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询该作词人的歌曲：{0}", person);
            var res = await  _getLyricsWriterSongs(person);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^7] == '作' && query[^6]=='曲')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询该作曲人的歌曲：{0}", person);
            var res = await  _getSongWriterSongs(person);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        
        else if (query.Length>9 && query[^6] == '音' && query[^5] == '乐' && query[^4] == '专' && query[^3] == '辑')
        {
            var song = query.Remove(query.Length-9, 9);
            string s = String.Format("查询歌曲所属的音乐专辑：{0}", song);
            var res = await  _getAlbum(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^6] == '歌' && query[^5] == '曲' && query[^4] == '作' && query[^3] == '者')
        {
            var song = query.Remove(query.Length-7, 7);
            string s = String.Format("查询歌曲的歌曲作者：{0}", song);
            var res = await  _getSongWriters(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^6] == '歌' && query[^5] == '词' && query[^4] == '作' && query[^3] == '者')
        {
            var song = query.Remove(query.Length-7, 7);
            string s = String.Format("查询歌曲的歌词作者：{0}", song);
            var res = await  _getLyricsWriters(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>5 && query[^4] == '歌' && query[^3]=='手')
        {
            var song = query.Remove(query.Length-5, 5);
            string s = String.Format("查询歌曲的歌手：{0}", song);
            var res = await  _getSingers(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^6] == '详' && query[^5] == '细' && query[^4] == '信' && query[^3] == '息')
        {
            var song = query.Remove(query.Length-7, 7);
            string s = String.Format("查询歌曲详细信息：{0}", song);
            var res = await _getDetail(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n"+res;
            return StatusCode(201, s); 
        }
        
        return StatusCode(201, "请检查问句格式！");
    }

    private async Task<string[]> _getAlbum(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`所属专辑`]->(b) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getLyricsWriters(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`作词`]->(b) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getSingers(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`歌手`]->(b) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getSongWriters(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`作曲`]->(b) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getAlbumSongs(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`)-[:`所属专辑`]->(b:`音乐专辑`{`名字`:$name}) return a.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["a.名字"]);
        List<string> strList = new List<string>();
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getDate(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name}) return a.发行日期", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["a.发行日期"]);
        List<string> strList = new List<string>();
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getSingerSongs(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`)-[:`歌手`]-> (b:`歌手`{`名字`:$name}) return a.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["a.名字"]);
        List<string> strList = new List<string>();
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getSongWriterSongs(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`)-[:`作曲`]->(b:`歌曲作者`{`名字`:$name}) return a.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["a.名字"]);
        List<string> strList = new List<string>();
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }
    
    private async Task<string[]> _getLyricsWriterSongs(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`)-[:`作词`]->(b:`歌词作者`{`名字`:$name}) return a.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["a.名字"]);
        List<string> strList = new List<string>();
        var i = 1;
        foreach (string v in resultList)
        {
            strList.Add(i.ToString()+". "+v);//循环添加元素
            i = i + 1;
        }
        string[] strArray = strList.ToArray();
        if (strArray.Length == 0)
        {
            var res = new string[1]{"未找到相关信息"};
            return res;
        }
        return strArray;
    }

    private async Task<string> _getDetail(string song)
    {
        var dateRes = await _getDate(song);
        var date = dateRes[0];
        var albumRes = await _getAlbum(song);
        var album = albumRes[0];

        var singersRes = await _getSingers(song);
        var singers = "";
        foreach (var v in singersRes)
        {
            singers = singers + v +"\r\n";
        }
        
        var lyricsWritersRes = await _getLyricsWriters(song);
        var lyricsWriters = "";
        foreach (var v in lyricsWritersRes)
        {
            lyricsWriters = lyricsWriters + v +"\r\n";
        }
        
        var songWritersRes = await _getSongWriters(song);
        var songWriters = "";
        foreach (var v in songWritersRes)
        {
            songWriters = songWriters + v +"\r\n";
        }

        var res = "";
        res = res + "发行日期：" + date + "\r\n";
        res = res + "所属专辑：" + album + "\r\n";
        res = res + "歌手：" + "\r\n" + singers;
        res = res + "歌词作者："  + "\r\n"+ lyricsWriters;
        res = res + "歌曲作者："  + "\r\n"+ songWriters;
        return res;
    }
}
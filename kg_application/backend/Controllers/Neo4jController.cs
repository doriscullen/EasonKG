using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
using Neo4j.Driver;
using System.Text;

[Route("api/[controller]")] 
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
            string s = String.Format("查询专辑：{0}", album);
            return StatusCode(201, s); 
        }
        if (query[0] == '歌' && query[1]=='曲')
        {
            var song = query.Remove(0, 2);
            song = song.Remove(song.Length-7, 7);
            string s = String.Format("查询歌曲发行日期：{0}", song);
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^7] == '演' && query[^6]=='唱')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询歌手：{0}", person);
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^7] == '作' && query[^6]=='词')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询作词人：{0}", person);
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^7] == '作' && query[^6]=='曲')
        {
            var person = query.Remove(query.Length-7, 7);
            string s = String.Format("查询作曲人：{0}", person);
            return StatusCode(201, s); 
        }
        else if (query.Length>7 && query[^6] == '详' && query[^5] == '细' && query[^4] == '信' && query[^3] == '息')
        {
            var song = query.Remove(query.Length-7, 7);
            string s = String.Format("查询歌曲详细信息：{0}", song);
            return StatusCode(201, s); 
        }
        else if (query.Length>9 && query[^6] == '音' && query[^5] == '乐' && query[^4] == '专' && query[^3] == '辑')
        {
            var song = query.Remove(query.Length-9, 9);
            string s = String.Format("查询歌曲音乐专辑：{0}", song);
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
            string s = String.Format("查询歌曲歌曲作者：{0}", song);
            var res = await  _getSongWriter(song);
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
            string s = String.Format("查询歌曲歌词作者：{0}", song);
            var res = await  _getLyricsWriter(song);
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
            string s = String.Format("查询歌曲歌手：{0}", song);
            var res = await  _getSinger(song);
            s = s + "\r\n" + "搜索结果：" + "\r\n";
            foreach (var v in res)
            {
                s = s + v +"\r\n";
            }
            return StatusCode(201, s); 
        }
        
        return StatusCode(201, "无匹配");
    }

    private async Task<string[]> _getAlbum(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`所属专辑`]->(b:`音乐专辑`) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        return strArray;
    }
    
    private async Task<string[]> _getSongWriter(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`作词`]->(b:`歌词作者`) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        return strArray;
    }
    
    private async Task<string[]> _getSinger(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`歌手`]->(b:`歌手`) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        return strArray;
    }
    
    private async Task<string[]> _getLyricsWriter(string name)
    {
        var statementParameters = new Dictionary<string, object> 
        { 
            {"name", name } 
        };
        
        var session = this._driver.AsyncSession();
        var result = await session.RunAsync("match (a:`歌曲`{`名字`:$name})-[:`作曲`]->(b:`歌曲作者`) return b.名字", statementParameters);
        var resultList = await result.ToListAsync(record =>record.Values["b.名字"]);
        List<string> strList = new List<string>(); 
        foreach (string v in resultList)
        {
            strList.Add(v);//循环添加元素
        }
        string[] strArray = strList.ToArray();
        return strArray;
    }
}
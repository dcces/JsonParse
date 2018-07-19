# JsonParse
## 用途
可以把json格式文件插入到数据库,需要先创建数据库表,字段需和json文件里的一致。

## 用法

```
FileStream fs = new FileStream("../../areas.json", FileMode.Open, FileAccess.Read);
string connstr = "server=127.0.0.1;user id=sa;password=1234;database=BK";
Parse p = new Parse(fs, connstr, "Area", 1024*10, Encoding.UTF8);//文件流  连接字符串  表名  缓冲区大小 编码格式
p.start();
```

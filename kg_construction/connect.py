from py2neo import Graph, Node, Relationship
import pandas as pd

#connect neo4j, username=neo4j, password=ZJUKG2022
graph = Graph("neo4j://localhost:7687", auth=("neo4j", "ZJUKG2022"))
graph.run('match (n) DETACH DELETE n')

#read in data
relation_data = pd.read_csv('../data_collection/data/relation.csv')
relation_data.columns = ['obj1', 'relation', 'obj2']
date_data = pd.read_csv('../data_collection/data/song_date.csv')
date_data.columns = ['song', 'attribute', 'value']

songs = {}
objs = {}
for _, row in relation_data.iterrows():
    songs[row['obj1']] = None
    objs[row['obj2']] = None

#create node
for k, v in songs.items():
    songs[k] = Node('歌曲', 名字=k)
for k, v in objs.items():
    objs[k] = Node('人物', 名字=k)

for _, row in date_data.iterrows():
    songs[row['song']][row['attribute']] = row['value']

#change relation
for index, row in relation_data.iterrows():
    objs[row['obj2']].clear_labels()
    if row['relation'] == '所属专辑':
        objs[row['obj2']].clear_labels()
        objs[row['obj2']].add_label('音乐专辑')
    elif row['relation'] == '作词':
        objs[row['obj2']].add_label('歌词作者')
    elif row['relation'] == '作曲':
        objs[row['obj2']].add_label('歌曲作者')
    else:  # 歌手
        objs[row['obj2']].add_label('歌手')
    r = Relationship(songs[row['obj1']], row['relation'], objs[row['obj2']])
    graph.create(r)
    print(index)

# g=graph.run('match (n) return n')
# for node  in g:
#     print(node)

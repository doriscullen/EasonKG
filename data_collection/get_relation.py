from paddlenlp import Taskflow
import pandas as pd

schema = ['歌曲', '歌手', '歌词作者', '歌曲作者']    # Define the schema for entity extraction
ie = Taskflow('information_extraction', schema=schema)

schema = {'歌曲名称': ['歌手', '所属专辑', '作词', '作曲']}  
ie.set_schema(schema)

df = pd.read_csv("data/baike.csv")
intros = df['歌曲介绍'].to_list()
songs = df['歌曲名称'].to_list()

relation_results = []

for i in range(0, len(intros)):
    num_songs = len(intros)
    progress = '['+ str(i+1) + '/' + str(num_songs) + ']'
    
    intro = intros[i]
    result = ie(intro)

    relations = []

    try:
        result = result[0]
        result = result['歌曲名称'][0]
        song_name = result['text']
        relations = result['relations']
    except:
        print(progress, 'Get relation failed')
    
    if relations != []:
        print(progress, song_name)
        for r_name in ['歌手', '所属专辑', '作词', '作曲']:
            try:
                items = relations[r_name]
                for item in items:
                    relation_results.append([song_name, r_name, item['text']])
            except:
                print(progress, song_name, r_name, 'not found')   

df_relation = pd.DataFrame(columns = ['', '', ''], data = relation_results)
df_relation.drop_duplicates(subset=None, keep='first', inplace=True, ignore_index=False)

print(df_relation)
df_relation.to_csv('data/relation.csv')
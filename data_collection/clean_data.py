import pandas as pd
import html

df = pd.read_csv("data/relation.csv")
# df = pd.read_csv("data/song_date.csv")
df.columns=['index','item1','relation','item2']

df.set_index('index', inplace=True)
df = df.sort_values(by = "item1")
df = df.reset_index(drop=True)

for i in range(0, df.shape[0]):
    for j in range(0, 3):
        item = df.iloc[i, j]
        item = item.replace('《', '')
        item = item.replace('》', '')
        item = html.unescape(item)
        df.iloc[i, j] = item

    for j in range(0, 3):
        item = df.iloc[i, j]
        item = html.unescape(item)
        df.iloc[i, j] = item

print(df)
df.to_csv("data/relation.csv", header=False, index=False)
# df.to_csv("data/song_date.csv", header=False, index=False)

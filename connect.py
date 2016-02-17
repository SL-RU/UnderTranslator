#!/usr/bin/python3

#In folder must be file translate.txt and files translate_*.txt

import glob, sys

orig = "translate.txt"
nams = glob.glob("./translate_*.txt")
strg = "STRG.txt"

sorig = list()
count = 0
sstrg = list()

trans = list()

stat = list()
stat.append(0) #strg
stat.append(0) #orig

with open(orig) as f:
    for i in f:
        sorig.append(i)
        count += 1

with open(strg) as f:
    for i in f:
        sstrg.append(i)

for tr in nams:
    j = 0
    cu = list()
    stat.append(0)
    with open(tr) as f:
        for i in f:
            cu.append(i)
            j += 1
    if j != count:
        print("Wrong lines count in " + tr)
        sys.exit()
    trans.append(cu)

fin = list()

def resolve(i, d):
    global trans, sstrg, strg, sorig, orig, nams, stat, fin
    print("RESOLVE:\n" + strg + " [0]: " + sstrg[i])
    print(orig + " [1]: " + sorig[i])
    for j in d:
        print(nams[j] + " [" + str(j + 2) + "]: " + trans[j][i])
    print("Select: ")
    s = int(input())
    if s < 0 or s > len(d) + 2:
        print("wrong number")
        resolve()
    else:
        p = ""
        if s == 0:
            p = sstrg[i]
        elif s == 1:
            p = sorig[i]
        else:
            p = trans[s-2][i]
        stat[s] += 1
        fin.append(p)

for i in range(count):
    d = list()
    for c in range(len(trans)):
        if trans[c][i] != sorig[i]:
            d.append(c)
    if len(d) > 1:
        resolve(i, d)
    elif len(d) == 1:
        fin.append(trans[d[0]][i])
        stat[d[0] + 2] += 1
    else:
        fin.append(sorig[i])
        stat[1] += 1

with open("final.txt", "w") as f:
    for i in fin:
        f.write(i)

print("DONE")
print("count:\t\t\t" + str(count))
print("stat: ")
print(strg + ":\t\t" + str(stat[0]))
print(orig + ":\t\t" + str(stat[1]))
for d in range(len(nams)):
    print(nams[d] + ":\t" + str(stat[d+2]))

#!/usr/bin/python3

import os, sys

lns = list()

i = 0

with open("ImportText.csv", encoding='utf-16') as f:
    for l in f:
        q = ()
        i+=1
        if "\t" in l:
            q = l.split("\"\t\"")
        else:
            q = l.split("\" \"")


        if len(q) == 1:
            print(str(i) + ": " + l)
        else:
            q[0] = q[0][1:]
            q[1] = q[1][:len(q[1]) - 2]
            lns.append((q[0], q[1]))
            if i < 30:
                print(str(i) + ": " + q[0] + "><" + q[1] + "|||")



print("csv ok")

las = 0
j = 0
i = 0

strg = list()

with open("STRG.txt", encoding='utf-8') as f:
    for l in f:
        l = l.replace("\n", "")
        #l = l[:len(l) - 2]
        strg.append(l)
        i += 1
        if i < 30:
            print(l)
        #print(l)
i = 0
truse = 0
with open("translate.txt", 'w', encoding='utf-16') as f:
    for l in strg:
        ok = 0

        for g in lns:
            if l == g[0]:
                f.write(g[1] + "\n")
                truse += 1
                ok = 1
                break
        if ok == 0:
            f.write(l + "\n")
        i += 1
        if i%500 == 0:
            print(str(i) + "/" + str(len(strg)))


print("DONE")
print("Used trans: " + str(truse) + " from " + str(len(lns)))
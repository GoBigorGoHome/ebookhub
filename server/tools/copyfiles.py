import os
import sys
import shutil
from glob import glob
import argparse

def doCopy(srcDir, dstDir, fileTypes):
    print "Copying files of type {0} from {1} to {2}".format(fileTypes, srcDir, dstDir)

    filesToCopy = []
    for dirpath, dirname, files in os.walk(srcDir):
        for item in files:
            for ext in fileTypes:
                if item.endswith(ext):
                    filesToCopy.append(str(os.path.join(dirpath,item)))

    total = len(filesToCopy)

    for idx in range(len(filesToCopy)):
        shutil.copy(filesToCopy[idx], dstDir)
        __updateProgress((idx+1) / float(total) * 100.0)

def __updateProgress(progress):
    sys.stdout.write('\r[{0}] {1}%'.format('#'*(int(progress)/10), int(progress)))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("src", help="source folder")
    parser.add_argument("dst", help="destination folder")
    parser.add_argument("-t", "--types", help="File types to copy (e.g. .txt .epub)", nargs="+")
    args = parser.parse_args()

    doCopy(args.src, args.dst, args.types)

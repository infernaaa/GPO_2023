import sys
import numpy as np
import pandas as pd

for i,item in zip(range(len(sys.argv)),sys.argv):
	print(f"Arg [{i}]: ", item)

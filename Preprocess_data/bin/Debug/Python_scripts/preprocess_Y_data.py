import sys
import numpy as np
import pandas as pd


path_to_defects = sys.argv[1]
path_to_save_defects = sys.argv[2]
rows_number = int(sys.argv[3])
detectors_number = int(sys.argv[4])


def get_df(path: str):
    df = pd.read_csv(path, delimiter=';')
    return df


first_defects_df = get_df(path_to_defects)
first_defects_df = first_defects_df.loc[:,'row_min':'detector_max']
defects_df = pd.concat([pd.Series([0 for i in range(detectors_number)]) for i in range(rows_number)], 
        ignore_index=True, axis=1).T
defects_df.columns = [f'detector_{i}' for i in range(defects_df.shape[1])]


def get_row_defects_dataframe(row_df, orig_df):
    temp_df = orig_df
    row_min = row_df.loc['row_min']
    row_max = row_df.loc['row_max'] + 1
    detector_min = row_df.loc['detector_min']   
    detector_max = row_df.loc['detector_max']     
    

    if (detector_min < detector_max):
        result_df = temp_df.iloc[row_min:row_max,detector_min:detector_max+1]
    else:
        result_df = pd.concat(
            [temp_df.iloc[row_min:row_max,:detector_max+1],
             temp_df.iloc[row_min:row_max,detector_min:]], axis=1)
    result_df[:] = 1
    return result_df

for row_name in first_defects_df.index.values.tolist():
    temp_df = get_row_defects_dataframe(first_defects_df.loc[row_name], defects_df)

defects_df.index.name = 'Row'
defects_df.to_numpy(dtype='bool')
defects_df = defects_df.T
test = defects_df[defects_df == 1].count()
test = test[test > 0]
defects_df.to_excel(path_to_save_defects)  

print('boba')
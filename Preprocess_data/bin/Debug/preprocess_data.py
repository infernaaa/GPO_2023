import sys
import pandas as pd
import numpy as np
import psycopg2
import datetime

print('Библиотеки подключены')

for i,item in zip(range(len(sys.argv)),sys.argv):
	print(f"Arg [{i}]: ", item)

# Константы пути

path_to_X_data = sys.argv[1]
path_to_Y_data = sys.argv[2]

# Константы подключения к бд

db_name = 'gpo_2023'
pg_host = 'localhost'
sql_user = 'postgres'
sql_pwd = '12345'
port = '5432'
schema_name = 'public'

# Константы запросов

insert_in_import_files_x_query = """INSERT INTO public.import_file_x (id, file_name, create_date) VALUES (%s, %s, %s)"""
insert_in_import_files_y_query = """INSERT INTO public.import_file_y (id, file_name, create_date) VALUES (%s, %s, %s)"""

insert_in_bool_measure = """INSERT INTO public.bool_measure
    (row_id, detector_id, file_id, defect_state) VALUES"""
insert_in_array_measure = """INSERT INTO public.array_measure
    (row_id, detector_id, file_id, time_values, amplitude_values) VALUES"""

def get_df(path: str):
    df = pd.read_csv(path, delimiter=';')
    return df

def to_list_of_64_values(x):
  # делим начальную строку на пары чисел
  # пары чисел тут - это просто строки
  x = str(x).split(',')

  # делим пары чисел из строк и преобразуем в numpy
  x = list(map(lambda y: str(y).split(':'), x))
  x = np.array(x)

  # если в ячейке не было данных
  if x.shape[0] == x.shape[1] == 1:
    x = np.zeros(64).astype(float)
    return x

  # если где то у числа не будет хватать пары
  if x.shape[1] != 2:
    print(x.shape)
    print('\nerror')
    x = np.zeros(64).astype(float)
    return x

  # добавляем числа-заполнители чтобы в каждой ячейке итогового датасета было по 64 числа
  x = np.pad(x, ((0,32 - len(x)),(0,0)), 'constant', constant_values=(0))

  # меняем располоение числел в итоговом массиве
  # теперь они не идут попарно (время, амплитуда)
  # а 1 измерение, где первые 32 числа - значения времени,
  # а вторые 32 - амплитуды
  left_x = x[:,0]
  right_x = x[:,1]
  x = np.concatenate((left_x, right_x), axis=0)
  x = x.astype(float)

  return x

def to_str(x):
  return np.array2string(x, precision=4, separator=',',floatmode='fixed')[1:-1]

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

print('Чтение данных')

# Чтение данных из файлов
first_X_df = get_df(path_to_X_data)
first_Y_df = get_df(path_to_Y_data)

print('Обработка данных')

# Первичная обработка X данных
first_X_df = first_X_df.drop(['position'], axis=1)
first_X_df['row'] = first_X_df['row'].astype(int)
first_X_df = first_X_df.set_index('row')

# Первичная обработка Y данных
first_Y_df = first_Y_df.loc[:,'row_min':'detector_max']

# важные переменные
PREP_rows_count = first_X_df.shape[0]
PREP_detectors_count = first_X_df.shape[1]

# теперь в каждой ячейке находится одномерный массив из 64 значений
# первые 32 - значения времени (если в оригинале значений было меньше 32, то к ним дописаны нули)
# вторые 32 - значения амплитуды (если в оригинале значений было меньше 32, то к ним дописаны нули)
X_df = first_X_df.apply(lambda x: x.apply(to_list_of_64_values, convert_dtype=True))

# создание датафрейма определенного вида для записи в него показателей дефектов
Y_df = pd.DataFrame(np.zeros((PREP_rows_count,PREP_detectors_count)))

Y_df.columns = [f'detector_{i}' for i in range(Y_df.shape[1])]

# обработка Y данных
for row_name in first_Y_df.index.values.tolist():
    temp_df = get_row_defects_dataframe(first_Y_df.loc[row_name], Y_df)

    # наложить один датафрейм на другой с учетом имен столбцов и индектов
    Y_df.loc[temp_df.index.values.tolist(),list(temp_df.columns)] = temp_df

Y_df.index.name = 'Row'
Y_df.to_numpy(dtype='bool')

print('Подключение к бд')

# подключение к бд
conn = psycopg2.connect(database=db_name,
                        host=pg_host,
                        user=sql_user,
                        password=sql_pwd,
                        port=port)
with conn:
    with conn.cursor() as cursor:
        cursor.execute('SET search_path to ' + schema_name)

print('Заполнение бд')

# проверка того, сколько записей в таблице import_file_x и import_file_x
result = 0

with conn:
    with conn.cursor() as cursor:
        cursor.execute('SELECT id FROM public.import_file_x')
        result = cursor.fetchall()

if len(result) == 0:
    last_file_id = 0
else:
    last_file_id = result[-1][0] + 1

# вставка инфы в import_files_x и import_files_x
with conn:
    with conn.cursor() as cursor:
        cursor.execute(insert_in_import_files_x_query,
                       (last_file_id,
                        path_to_X_data.split('\\')[-1],
                        datetime.date.today().strftime("%Y-%m-%d")))

with conn:
    with conn.cursor() as cursor:
        cursor.execute(insert_in_import_files_y_query,
                       (last_file_id,
                        path_to_Y_data.split('\\')[-1],
                        datetime.date.today().strftime("%Y-%m-%d")))

conn.commit()

# вставка инфы в bool_measure
for i in range(PREP_rows_count):
    temp_i_arr = np.full((PREP_detectors_count), i)
    temp_j_arr = np.arange(PREP_detectors_count)
    temp_file_id_arr = np.full((PREP_detectors_count), last_file_id)
    temp_i_defect_state_arr = Y_df.iloc[i,:].to_numpy().astype(bool)

    temp_tuple = list(zip(temp_i_arr, temp_j_arr, temp_file_id_arr, temp_i_defect_state_arr))

    temp_tuple_str = ','.join(['(' + ','.join([str(item) for item in row]) + ')' for row in temp_tuple])

    with conn:
        with conn.cursor() as cursor:
            cursor.execute(insert_in_bool_measure + temp_tuple_str)

conn.commit()

# вставка инфы в array_measure
for i in range(PREP_rows_count):
    temp_i_arr = np.full((PREP_detectors_count), i)
    temp_j_arr = np.arange(PREP_detectors_count)
    temp_file_id_arr = np.full((PREP_detectors_count), last_file_id)
    temp_i_time_values_arr = ['\'{' + to_str(item) + '}\'' for item in X_df.iloc[i,:][:32]]
    temp_i_amplitude_values_arr = ['\'{' + to_str(item) + '}\'' for item in X_df.iloc[i,:][32:]]

    temp_tuple = list(zip(temp_i_arr, temp_j_arr, temp_file_id_arr,
                          temp_i_time_values_arr, temp_i_amplitude_values_arr))

    temp_tuple_str = ','.join(['(' + ','.join([str(item) for item in row]) + ')' for row in temp_tuple])

    with conn:
        with conn.cursor() as cursor:
            cursor.execute(insert_in_array_measure + temp_tuple_str)

conn.commit()

cursor.close()
conn.close()

print('Данные успешно обработаны и записаны')

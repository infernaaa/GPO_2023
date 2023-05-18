import os
import numpy as np
import pandas as pd
import sys

path_to_data = sys.argv[1] # расположение всех измерений
# куда сохранять X часть выборки
# ячейки таблицы - массивы
path_to_save_data_array_like = sys.argv[2] # куда сохранять X часть выборки
# ячейки таблицы - числа, для отображения всех данных используются сложные индексы
def get_df(path: str):
    df = pd.read_csv(path, delimiter=';')
    return df

# Чтение данных из файла
first_df = get_df(path_to_data)
# Что прочли из файла


# Удаляем лишние колонки, назначаем новый индекс
first_df = first_df.drop('position', axis=1)
first_df['row'] = first_df['row'].astype(int)
first_df = first_df.set_index('row')

# важные переменные
rows_count = first_df.shape[0]
detectors_count = first_df.shape[1]


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
prepared_df = first_df.apply(lambda x: x.apply(to_list_of_64_values, convert_dtype=True))
# теперь в каждой ячейке находится одномерный массив из 64 значений
# первые 32 - значения времени (если в оригинале значений было меньше 32, то к ним дописаны нули)
# вторые 32 - значения амплитуды (если в оригинале значений было меньше 32, то к ним дописаны нули)

# теперь так как в каждой ячейке массив - можно довольно удобно обращаться к его членам
# тут выведен срез из первых 32 элементов массива из ячейки 0 строки 0 столбца итогового 
# датафрейма

def to_str(x):
  return np.array2string(x, precision=4, separator=',',floatmode='fixed')[1:-1]
# СОХРАНЯЕМ ДАТАСЕТ В ФАЙЛ
# где каждая ячейка - массив
array_df = prepared_df.apply(lambda x: x.apply(to_str, convert_dtype=True))
# СОХРАНЯЕМ ДАТАСЕТ В ФАЙЛ
# где каждая ячейка - массив
array_df.to_excel(path_to_save_data_array_like)  

print('biba')
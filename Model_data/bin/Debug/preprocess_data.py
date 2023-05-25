import sys
import pandas as pd
import numpy as np
import psycopg2
import datetime

print('Библиотеки подключены')

for i,item in zip(range(len(sys.argv)),sys.argv):
	print(f"Arg [{i}]: ", item)

# Константы пути

id_file = sys.argv[1]
id_row = sys.argv[2]
id_detector = sys.argv[3]

# Константы подключения к бд

db_name = 'gpo_2023'
pg_host = 'localhost'
sql_user = 'postgres'
sql_pwd = '12345'
port = '5432'
schema_name = 'public'

# Константы запросов

get_table_query = """
SELECT array_measure.time_values, array_measure.amplitude_values
	FROM
	array_measure JOIN import_file_x
	ON array_measure.file_id = import_file_x.id
	WHERE
		import_file_x.id = %s,
		array_measure.row_id = %s,
		array_measure.detector_id = %s;"""

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


result = ""
# вставка инфы в import_files_x и import_files_x
with conn:
    with conn.cursor() as cursor:
        cursor.execute(get_table_query,
                       (id_file, id_row, id_detector))
	    result = cursor.fetchall()


print(result)

conn.commit()
cursor.close()
conn.close()

print('Данные успешно прочитаны')

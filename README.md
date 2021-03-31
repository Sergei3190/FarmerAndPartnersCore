# FarmerAndPartnersCore

Небольшое настольное WPF-приложение.

Технические требования к системе:

- Приложение: WPF/MVVM 
- Платформа: .NET Core 3.1
- База данных: MSSQL
- ORM: Entity Framework Core

Есть пользователь (User) со свойствами Id, Name, Login, Password
Есть компания (Company) со свойствами Id, Name, ContractStatus
ContractStatus - статус договора на услуги с компанией, принимает значения: Ещё не заключен, Заключен, Расторгнут
В компании может быть несколько пользователей
Пользователей без компании не существует

Требования к функционалу приложения:

- Создавать/Редактировать/Удалять компании
- Создавать/Редактировать/Удалять пользователей
- Отображать списки компаний и пользователей компании
- Компания отображает список относящихся к ней пользователей
- Изменения/добавления компании/пользователя должны синхронизироваться, если компании и пользователи располагаются в разных вкладках интерфейса пользователя
- Производить проверку на параллелизм
- Производить проверку достоверности введенных пользователем данных
- Блокировать создание/редактирование компании/пользователя при наличии ошибок ввода  
- Сохранять/загружать списки компаний и пользователей в/из файл(а) в форматах XML/Json
- Интерактивность
- Расширяемость
- Загрузка больших массивов данных из бд без блокирования интерфейса пользователя
- Логирование в файл

PS: после инициализации БД с помощью миграций необходимо выполнить скрипт StoredProcedures.sql (путь: FarmerAndParthers/FarmerAndPartnersEF/EF/Scripts/)

# RConfig - простой импорт данных из Google таблиц в Unity

## Цели

* Иметь удобное API для получения данных из гугл таблиц в Unity
* Иметь возможность обновлять данные по ходу исполнения программы

## Краткий обзор API

Данные описываются с помощью RCScheme

``` c#
public class UnitScheme : RСScheme
{
    public RCType Health;
    public RCType Name;
    public RCType OtherData;
}
```

| Key        | Health | Name   | OtherData   |
|:-----------|:-------|:-------|:------------|
| unit_one   | 5      | Mario  | 0.3         |
| unit_two   | 6      | Luigi  | true        |
| unit_three | 10     | Bowser | Hello World |

Получение данных

```c#
var unitScheme = RConfig.Get<UnitScheme>("unit_one");
var health = unitScheme.Health.ToInt(); // 5
var name = unitScheme.Name; // Mario
var floatValue = unitScheme.OtherData.ToFloat(); // 0.3f
```

Данные можно закэшировать и использовать в качестве переменных

```c#
private _unitTwoScheme = new RCVar<UnitScheme>("unit_two");
private _unitThreeScheme = new RCVar<UnitScheme>("unit_three");

void UseData()
{
	var unitTwoName = _unitTwoScheme.Get().Name; // Luigi
	var unitThreeName = _unitThreeScheme.Get().Name; // Bowser
}
```

## Установка
> **ВАЖНО!** Если вы используете версию Unity ниже чем 2021.3 работоспособность не гарантируется.

### В виде unity модуля
Поддерживается установка в виде unity-модуля через git-ссылку в PackageManager или прямое редактирование `Packages/manifest.json`:
```
"com.andrey_birchenko.rconfig": "https://github.com/AndreyBirchenko/RConfig.git",
```

## Начало работы
> **ВАЖНО!** Не рекомендуется использовать скачивание данных с GoogleDocs в релизных билдах:
> * Время отклика может достигать десятка секунд.
> * Лимит по обращению к документу может быстро переполниться и документ будет заблокирован на какое-то время.

### Создание таблицы

Для начала вам понадобится создать гугл таблицу. Первый столбец это всегда ключ по которому будет производится поиск, в
остальных столбцах данные

| Key          | ValueOne | ValueTwo |
|:-------------|:---------|:---------|
| float_values | 2.14     | 9.8      |
| int_values   | 6        | 12       |
| bool_values  | true     | false    |

> Вы также можете создавать несколько страниц в одной таблице.

После этого [откройте доступ к таблице](https://support.google.com/docs/answer/9331169?hl=ru#6.1) по публичной ссылке

### Создание схемы

Схема это представление данных вашей таблицы в C# коде. Создайте класс MyCustomScheme по примеру ниже.

```c#
public class MyCustomScheme : RCScheme
{
    public RCType ValueOne;
    public RCType ValueTwo;
}
```

Важно чтобы для каждого столбца кроме ключа было создано поле.

Для удобства пакет уже содержит схему ```KeyValueScheme```
Она подойдёт для таблиц вида

| Key       | Value |
|:----------|:------|
| value_one | 2.14  |
| value_two | true  |

### Настройка

После установки пакета в папке Resources автоматически создался объект RCData.

С помощью него осуществляется привязка схем к таблицам и обновление данных.

Нажмите кнопку AddScheme и выберите в выпадающем окне схему с названием MyCustomScheme. Скопируйте ссылку на таблицу из
адресной строки браузера, вставьте в поле Page url и нажмите Update data.

Если вы всё сделали правильно в консоли появится лог ```Updated MyCustomScheme```

## Использование

### Чтение данных

Есть несколько способов чтение данных из схемы выбирайте тот что будет удобнее

```c#
// Чтение напрямую из схемы
var myCustomScheme = RConfig.Get<MyCustomScheme>("float_values");
var floatValue = myCustomScheme.ValueOne.ToFloat(); // 2.14

// Создание поля и чтение из него
private _myCustomScheme = new RCVar<MyCustomScheme>("float_values");

void Update()
{
    var floatValue = myCustomScheme.Get().ValueOne.ToFloat();
}
```

### Обновление по ходу исполнения программы

Есть возможность обновлять данные как синронно так и асинхронно

```c#
private _myCustomScheme = new RCVar<MyCustomScheme>("float_values");

void Start()
{
    RConfig.DataUpdated += HandleDataUpdated;
    RConfig.UpdateData();
}

void HandleDataUpdated()
{
    // Используем обновлённые данные
    var floatValue = myCustomScheme.Get().ValueOne.ToFloat();
}
```

```c#
private _myCustomScheme = new RCVar<MyCustomScheme>("float_values");

async void Start()
{
    await RConfig.UpdateDataAsync();
    // Данные обновились и их можно использовать
    var floatValue = myCustomScheme.Get().ValueOne.ToFloat();
}

```
# RConfig - easy import of data from Google tables to Unity

## Goals

* Have a convenient API for getting data from Google tables in Unity
* Be able to update data at runtime

## A brief overview of the API

The data is described using RCScheme

```c#
public class UnitScheme : RCScheme
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

Getting data

```c#
var unitScheme = RConfig.Get<UnitScheme>("unit_one");
var health = unitScheme.Health.ToInt(); // 5
var name = unitScheme.Name; // Mario
var floatValue = unitScheme.OtherData.ToFloat(); // 0.3f
```

The data can be cached and used as variables

```c#
private _unitTwoScheme = new RCVar<UnitScheme>("unit_two");
private _unitThreeScheme = new RCVar<UnitScheme>("unit_three");

void UseData()
{
    var unitTwoName = _unitTwoScheme.Get().Name; // Luigi
    var unitThreeName = _unitThreeScheme.Get().Name; // Bowser
}
```

## Installation

> **IMPORTANT!** Functionality is not guaranteed for Unity versions below 2021.3.

### As a unity module

Installation is supported as a Unity module via a git link in the Package Manager or by directly editing `Packages/manifest.json`:

```
"com.andrey_birchenko.rconfig": "https://github.com/AndreyBirchenko/RConfig.git",
```

## Getting started

> **IMPORTANT!** It is not recommended to use downloading data from GoogleDocs in release builds:
> * Response time can reach ten seconds.
> * The limit on accessing a document can quickly overflow and the document will be blocked for a while.

### Creating a table

First, you need to create a Google Sheet. The first column is always the key for the lookup, and the other columns contain the data

| Key          | ValueOne | ValueTwo |
|:-------------|:---------|:---------|
| float_values | 2.14     | 9.8      |
| int_values   | 6        | 12       |
| bool_values  | true     | false    |

> You can also create multiple sheets within one document.

After that [share the document](https://support.google.com/docs/answer/9331169?hl=en#6.1)

### Creating a Scheme

A scheme is a representation of table data in C# code.

```c#
public class MyCustomScheme : RCScheme
{
    public RCType ValueOne;
    public RCType ValueTwo;
}
```

> **IMPORTANT!** There must be a field for each column except the key.

For convenience, the package already contains the `KeyValueScheme` scheme. It is suitable for tables like this

| Key       | Value |
|:----------|:------|
| value_one | 2.14  |
| value_two | true  |

### Setting up

Create a file in the Resources folder with the name RCData.txt

This file describes the correspondence of the Google table to the schemas

```
# The lines that start with # are comments
# Add entries in the SCHEME_NAME URL format
# Each diagram should be from a new line
# There should be a space between the scheme and the link

KeyValueScheme https://docs.google.com/spreadsheets/d/XXXX/edit#gid=XXXX
MyCustomScheme https://docs.google.com/spreadsheets/d/XXXX/edit#gid=XXXX

# You can use the same schemas for different data
KeyValueScheme https://docs.google.com/spreadsheets/d/YYYY/edit#gid=YYYY
```

## Usage

### Reading data

There are several ways to read data from the schema, choose the one that will be more convenient

```c#
// Reading directly from the schema
var myCustomScheme = RConfig.Get<MyCustomScheme>("float_values");
var floatValue = myCustomScheme.ValueOne.ToFloat(); // 2.14

// Creating a field and reading from it
private _myCustomScheme = new RCVar<MyCustomScheme>("float_values");

void Update()
{
    var floatValue = myCustomScheme.Get().ValueOne.ToFloat();
}
```

### Updating during runtime

It is possible to update data asynchronously

```c#
private _myCustomScheme = new RCVar<MyCustomScheme>("float_values");

async void Start()
{
    await RConfig.UpdateDataAsync();
    // The data has been updated and can be used
    var floatValue = myCustomScheme.Get().ValueOne.ToFloat();
}
```
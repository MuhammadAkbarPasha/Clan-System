
function GetServerData(keys)
{
    try
    {
        var itemData = server.GetTitleData(
        {
            Keys  : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetServerData(key, data)
{
    try
    {
        var itemData = server.SetTitleData(
        {
            Key : key,
            Value: data
        });
    }
    catch(err)
    {
        LogError({
            "EventName" : "SetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetServerInternalData(keys)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            Keys : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function GetSpecificUserInternalData(keys, playerId)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function SetServerInternalData(key, data)
{
    try
    {
        var itemData = server.SetTitleInternalData(
        {
            Key : key,
            Value: data
        });


    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetSpecificUserData(specificPlayerId,key, data)
{
    var dataToSend = {}
    dataToSend[key] = data;
        try
        {
            var itemData = server.UpdateUserData(
            {
                PlayFabId : specificPlayerId,
                Data: dataToSend
            });
            log.info("set specific user!");
        }
        catch(err)
        {
            LogError({
                "EventName" : "SetSpecificUserData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}

function GetSpecificUserData(keys, playerId)
{
    try
    {
        var itemData = server.GetUserData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
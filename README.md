
# NotificationService API

API that allows users to send Notifications and manage them

## Features

- Background jobs
- Cache
- Azure Service Bus
- JWT Auth 
- Refresh Tokens
- Email sending with SMTP
- Push sending with Firebase
- SMS sending with SMSPlanetAPI


# Built With
* ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
* ![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)
* ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white) 
* ![Firebase](https://img.shields.io/badge/firebase-%23039BE5.svg?style=for-the-badge&logo=firebase)
* ![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=for-the-badge&logo=azure-devops&logoColor=white) 
* ![Terraform](https://img.shields.io/badge/terraform-%235835CC.svg?style=for-the-badge&logo=terraform&logoColor=white) 
* ![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)  
* ![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Sever-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
* ![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)

# System Architecture

![Screenshot](https://raw.githubusercontent.com/solowiejmaciej/NotificationService/master/ArchitectureDiagram.drawio.png)

## Hangfire

| Job Name        | Cron        |
|:----------------|:------------|
| `Delete Sms`    | `* * * * *` |
| `Delete Emails` | `* * * * *` |
| `Delete Pushes` | `* * * * *` |

## Configuration
```json
{
  "ApiKeySettings": {
    "HeaderName": "x-api-key",
    "ApiKey": "youkey"
  },
  "AzureServiceBusSettings": {
    "ConnectionString": "foo",
    "QueueName": "foo"
  },
  "SmsSettings": {
    "Key": "foo",
    "Password": "foo",
    "SenderName": "foo",
    "ApiUrl": "foo"
  },
  "GoogleFirebaseSettings": {
    "type": "foo",
    "project_id": "foo",
    "private_key_id": "foo",
    "private_key": "foo",
    "client_email": "foo",
    "client_id": "foo",
    "auth_uri": "foo",
    "token_uri": "foo",
    "auth_provider_x509_cert_url": "foo",
    "client_x509_cert_url": "foo",
    "universe_domain": "foo"
  },
  "RedisSettings": {
    "Endpoints": "foo",
    "Password": "foo"
  },
  "AuthSettings": {
    "PublicKey": "foo",
    "ExpireMinutes": 120,
    "Issuer": "foo"
  },
  "HangfireSettings": {
    "UserName": "foo",
    "Password": "foo"
  },
  "ConnectionStrings": {
    "App": "foo",
    "Hangfire": "foo",
    "Redis" : "foo"
  },
  "SMTPSettings": {
    "Host": "foo",
    "Port": 465,
    "UseSsl": true,
    "Username": "foo",
    "Password": "foo",
    "SenderEmail": "foo",
    "SenderName": "foo"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Hangfire": "Information"
    }
  },
  "AllowedHosts": "*"
}


```

## Auth


| Header          | Usage                | Required                    |
|:----------------|:---------------------|:----------------------------|
| `x-api-key`     | `Key to the api`     | `Required by every enpoint` |
| `Authorization` | `Token to auth user` | `Required by every enpoint` |
                    


## Usage example

```http
  POST /api/Auth/Register
```
#####
```curl
curl -X 'POST' \
  'https://localhost:7277/api/Auth/Register' \
  -H 'accept: */*' \
  -H 'X-Api-Key: youapikey' \
  -H 'Content-Type: application/json' \
  -d '{
  "firstname": "string",
  "surname": "string",
  "password": "string",
  "confirmPassword": "string",
  "email": "string@gmail.com",
  "phoneNumber": "111222333",
  "deviceId": "string"
}'
```

### Response

```json
{
  "token": "signedJwtTokenHere",
  "refreshToken": "2d29aedf-23d5-483c-ae75-58b235b52c5d",
  "statusCode": 200,
  "issuedDate": "2023-07-12T21:54:39.080989+02:00",
  "expiresAt": "2023-07-12T23:54:39.0714333+02:00",
  "role": "User",
  "roleId": "CAE53EE9-6E7C-44B4-B2B3-D062B4A346F8",
  "userId": "6cb70bb2-156b-42e5-9431-ffa960fe4d20"
}
```

## Send new Email

```http
  POST /api/Emails
```
#####
```curl
curl -X 'POST' \
  'https://localhost:7277/api/Emails?UserId=6cb70bb2-156b-42e5-9431-ffa960fe4d20' \
  -H 'accept: */*' \
  -H 'X-Api-Key: youapikey' \
  -H 'Content-Type: application/json' \
  -d '{
  "content": "string345",
  "subject": "string"
}'

```

### Response 

```json
{
  "subject": "string",
  "content": "string345",
  "recipiantId": "6cb70bb2-156b-42e5-9431-ffa960fe4d20"
}
```

## Get emails
```http
  GET /api/Emails
```
#####
```curl
curl -X 'GET' \
  'https://localhost:7277/api/Emails?Status=1' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer signedJwt' \
  -H 'X-Api-Key: youapikey'
```

### Response 

```json

{
  "items": [
    {
      "subject": "string",
      "id": 1003,
      "createdAt": "2023-07-12T21:57:38.3492636",
      "recipientId": "6cb70bb2-156b-42e5-9431-ffa960fe4d20",
      "status": 1,
      "content": "string345"
    }
  ],
  "totalPages": 1,
  "itemsFrom": 1,
  "itemsTo": 100,
  "totalItemsCount": 1
}

```
## Get email
```http
  GET /api/Emails/{id}
```
#####
```curl
curl -X 'GET' \
  'https://localhost:7277/api/Emails/1003' \
  -H 'accept: */*' \
  -H 'Authorization:  Bearer signedJwt' \
  -H 'X-Api-Key: youapikey'
```

### Response

```json

{
  "subject": "string",
  "id": 1003,
  "createdAt": "2023-07-12T21:57:38.3492636",
  "recipientId": "6cb70bb2-156b-42e5-9431-ffa960fe4d20",
  "status": 1,
  "content": "string345"
}

```

## Delete email
```http
  DELETE /api/Emails/{id}
```
#####
```curl
curl -X 'DELETE' \
  'https://localhost:7277/api/Emails/1003' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer signedJwt' \
  -H 'X-Api-Key: youapikey'
```

## Contact

- solowiejmaciej@gmail.com
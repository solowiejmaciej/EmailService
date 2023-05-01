
# EmailService API

Simple API that allowes users to send emails and manage them

## Features

- Background jobs
- Cache
- JWT Auth using AuthService
- Email sending with SMTP


# Built With
* ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
* ![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)
* ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
* ![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=for-the-badge&logo=azure-devops&logoColor=white) 
* ![Terraform](https://img.shields.io/badge/terraform-%235835CC.svg?style=for-the-badge&logo=terraform&logoColor=white) 
* ![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)  
* ![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Sever-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
* ![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)

## Hangfire

| Job Name  | Cron     |
| :-------- | :------- |
| `Add Test email to DB` | `never` |
| `Delete emails` | `never` |
| `Send background emails` | `* * * * *` |


![Screenshot](https://i.imgur.com/PcEkcQ5.png)

## Requirements
In order to use this API, you need to use JWT generated in [AuthService](https://github.com/solowiejmaciej/AuthService)


## Auth
Authorization Header with jwt token
## Fetch data

```http
  GET /api/Emails
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `creatorId` | `int` | Id of the account that created email |

### Response

```json
[
  {
    "id": 0,
    "emailSenderName": "string",
    "subject": "string",
    "emailTo": "string",
    "emailFrom": "string",
    "body": "string",
    "isEmailSended": true,
    "createdAt": "2023-05-01T13:50:32.432Z",
    "createdById": 0
  },
  {
    "id": 1,
    "emailSenderName": "string",
    "subject": "string",
    "emailTo": "string",
    "emailFrom": "string",
    "body": "string",
    "isEmailSended": true,
    "createdAt": "2023-05-01T13:50:32.432Z",
    "createdById": 0
  },
]
```

#### Get email

```http
  GET /api/Emails/${id}
```
### Response 

```json
{
  "id": 0,
  "emailSenderName": "string",
  "subject": "string",
  "emailTo": "string",
  "emailFrom": "string",
  "body": "string",
  "isEmailSended": true,
  "createdAt": "2023-05-01T13:51:35.299Z",
  "createdById": 0,
  "isDeleted": true
}
```

## Add new email
```http
  POST /api/Emails
```
### Body 

```json
{
  "emailSenderName": "string",
  "subject": "string",
  "emailTo": "email",
  "body": "string"
}
```

## Mark email as deleted


```http
  DELETE /api/Emails
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | **Required**|


## Add new email and send it now
```http
  POST /api/EmailSender/SendEmailNow
```
### Body
```json
{
  "emailSenderName": "string",
  "subject": "string",
  "emailTo": "string",
  "body": "string"
}
```
## Related

[AuthService](https://github.com/solowiejmaciej/AuthService)

## Contact

- solowiejmaciej@gmail.com

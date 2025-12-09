using System.Text.Json;
using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Links;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;
using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;
using Task = System.Threading.Tasks.Task;

namespace Planfact.AmoCrm.Client.Tests;

public class ApiContractVerifyTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true  
    };

    [Fact]
    public Task Should_Verify_Pipeline_Response()
    {
        // Arrange
        var response = new Pipeline
        {
            Id = 10195490,
            Name = "Воронка",
            Sort = 1,
            IsMain = true,
            IsUnsortedOn = true,
            IsArchive = false,
            AccountId = 32718250,
            AvailableStatuses = new PipelineStatusesContainer
            {
                Statuses =
                [
                    new LeadStatus
                    {
                        Id = 80731174,
                        Name = "Неразобранное",
                        Sort = 10,
                        IsEditable = false,
                        PipelineId = 10195490,
                        Color = "#c1c1c1",
                        Type = LeadStatusType.Unsorted,
                        AccountId = 32718250
                    },
                    new LeadStatus
                    {
                        Id = 80731246,
                        Name = "Неразобранное",
                        Sort = 10,
                        IsEditable = true,
                        PipelineId = 10195490,
                        Color = "#fffeb2",
                        Type = LeadStatusType.Regular,
                        AccountId = 32718250
                    }
                ]
            }
        };

        // Act & Assert
        return Verify(response)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Pipeline_Response_From_Generated_Json()
    {
        // Arrange
        var originalResponse = new Pipeline
        {
            Id = 10195490,
            Name = "Воронка",
            Sort = 1,
            IsMain = true,
            IsUnsortedOn = true,
            IsArchive = false,
            AccountId = 32718250,
            AvailableStatuses = new PipelineStatusesContainer
            {
                Statuses =
                [
                    new LeadStatus
                    {
                        Id = 80731174,
                        Name = "Неразобранное",
                        Sort = 10,
                        IsEditable = false,
                        PipelineId = 10195490,
                        Color = "#c1c1c1",
                        Type = LeadStatusType.Unsorted,
                        AccountId = 32718250
                    }
                ]
            }
        };

        var json = JsonSerializer.Serialize(originalResponse, _jsonOptions);

        // Act
        Pipeline? deserializedResponse = JsonSerializer.Deserialize<Pipeline>(json, _jsonOptions);

        // Assert
        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.AvailableStatuses.Should().NotBeNull();
        deserializedResponse.AvailableStatuses!.Statuses.Should().HaveCount(1);

        deserializedResponse.Id.Should().Be(originalResponse.Id);
        deserializedResponse.Name.Should().Be(originalResponse.Name);
        deserializedResponse.Sort.Should().Be(originalResponse.Sort);
        deserializedResponse.IsMain.Should().Be(originalResponse.IsMain);
        deserializedResponse.IsUnsortedOn.Should().Be(originalResponse.IsUnsortedOn);
        deserializedResponse.IsArchive.Should().Be(originalResponse.IsArchive);
        deserializedResponse.AccountId.Should().Be(originalResponse.AccountId);

        LeadStatus status = deserializedResponse.AvailableStatuses.Statuses[0];
        LeadStatus originalStatus = originalResponse.AvailableStatuses.Statuses[0];

        status.Id.Should().Be(originalStatus.Id);
        status.Name.Should().Be(originalStatus.Name);
        status.Sort.Should().Be(originalStatus.Sort);
        status.IsEditable.Should().Be(originalStatus.IsEditable);
        status.PipelineId.Should().Be(originalStatus.PipelineId);
        status.Color.Should().Be(originalStatus.Color);
        status.Type.Should().Be(originalStatus.Type);
        status.AccountId.Should().Be(originalStatus.AccountId);
    }

    [Fact]
    public void Should_Ignore_Pipeline_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 3177727,
            "some_new_field": "should_be_ignored",
            "name": "Воронка",
            "sort": 1,
            "is_main": true,
            "is_unsorted_on": true,
            "is_archive": false,
            "account_id": 12345678,
            "_embedded": {
                "statuses": [
                    {
                        "id": 32392156,
                        "name": "Неразобранное",
                        "extra_array": [1, 2, 3],
                        "sort": 10,
                        "is_editable": false,
                        "pipeline_id": 3177727,
                        "color": "#c1c1c1",
                        "type": 1,
                        "account_id": 12345678
                    }
                ]
            }
        }
        """;

        // Act
        Pipeline? response = JsonSerializer.Deserialize<Pipeline>(jsonWithExtraFields, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.AvailableStatuses.Should().NotBeNull();
        response.AvailableStatuses!.Statuses.Should().HaveCount(1);
        response.Id.Should().Be(3177727);
        response.Name.Should().Be("Воронка");
        response.Sort.Should().Be(1);
        response.IsMain.Should().BeTrue();
        response.IsUnsortedOn.Should().BeTrue();
        response.IsArchive.Should().BeFalse();
        response.AccountId.Should().Be(12345678);

        response.AvailableStatuses.Statuses[0].Id.Should().Be(32392156);
        response.AvailableStatuses.Statuses[0].Name.Should().Be("Неразобранное");
        response.AvailableStatuses.Statuses[0].Sort.Should().Be(10);
        response.AvailableStatuses.Statuses[0].IsEditable.Should().BeFalse();
        response.AvailableStatuses.Statuses[0].PipelineId.Should().Be(3177727);
        response.AvailableStatuses.Statuses[0].Color.Should().Be("#c1c1c1");
        response.AvailableStatuses.Statuses[0].Type.Should().Be(LeadStatusType.Unsorted);
        response.AvailableStatuses.Statuses[0].AccountId.Should().Be(12345678);
    }

    [Fact]
    public void Should_Handle_Pipeline_Response_Json_With_Null_Values()
    {
        // Arrange - JSON с null значениями (как может прийти от API)
        var jsonWithNulls = """
        {
            "id": 3177727,
            "some_new_field": "should_be_ignored",
            "name": null,
            "sort": 1,
            "is_main": true,
            "is_unsorted_on": true,
            "is_archive": false,
            "account_id": 12345678,
            "_embedded": {
                "statuses": [
                    {
                        "id": 32392156,
                        "name": "Неразобранное",
                        "extra_array": [1, 2, 3],
                        "sort": 10,
                        "is_editable": false,
                        "pipeline_id": 3177727,
                        "color": "#c1c1c1",
                        "type": 1,
                        "account_id": 12345678
                    }
                ]
            }
        }
        """;

        // Act
        Pipeline? response = JsonSerializer.Deserialize<Pipeline>(jsonWithNulls, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.AvailableStatuses.Should().NotBeNull();
        response.AvailableStatuses!.Statuses.Should().HaveCount(1);
        response.Id.Should().Be(3177727);
        response.Name.Should().BeNull();
        response.Sort.Should().Be(1);
        response.IsMain.Should().BeTrue();
        response.IsUnsortedOn.Should().BeTrue();
        response.IsArchive.Should().BeFalse();
        response.AccountId.Should().Be(12345678);

        response.AvailableStatuses.Statuses[0].Id.Should().Be(32392156);
        response.AvailableStatuses.Statuses[0].Name.Should().Be("Неразобранное");
        response.AvailableStatuses.Statuses[0].Sort.Should().Be(10);
        response.AvailableStatuses.Statuses[0].IsEditable.Should().BeFalse();
        response.AvailableStatuses.Statuses[0].PipelineId.Should().Be(3177727);
        response.AvailableStatuses.Statuses[0].Color.Should().Be("#c1c1c1");
        response.AvailableStatuses.Statuses[0].Type.Should().Be(LeadStatusType.Unsorted);
        response.AvailableStatuses.Statuses[0].AccountId.Should().Be(12345678);
    }

    [Fact]
    public Task Should_Verify_Contact_Response()
    {
        // Arrange
        var response = new Contact
        {
            Id = 12345678,
            Name = "Иванов Иван Иванович",
            FirstName = "Иван",
            LastName = "Иванов",
            ResponsibleUserId = 987654,
            ResponsibleUserGroupId = 111,
            CreatedBy = 987654,
            UpdatedBy = 987654,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            ClosestTaskAt = 1706745600,
            IsDeleted = false,
            AccountId = 32718250,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 12345,
                    FieldName = "Телефон",
                    FieldCode = "PHONE",
                    Values = [new CustomFieldValue { Value = "+79991234567" }]
                },
                new CustomFieldValuesContainer
                {
                    FieldId = 54321,
                    FieldName = "Статус клиента",
                    FieldCode = "CLIENT_STATUS",
                    Values =
                    [
                        new CustomFieldValue
                        {
                            Value = "Постоянный",
                            EnumId = 101,
                            EnumCode = "REGULAR"
                        }
                    ]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags =
                [
                    new Tag { Id = 101, Name = "Клиент" },
                    new Tag { Id = 102, Name = "VIP" }
                ],
                Companies =
                [
                    new Company { Id = 55555 }
                ]
            }
        };

        // Act & Assert
        return Verify(response)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Contact_Response_From_Generated_Json()
    {
        // Arrange
        var originalContact = new Contact
        {
            Id = 12345678,
            Name = "Петров Петр",
            FirstName = "Петр",
            LastName = "Петров",
            ResponsibleUserId = 987654,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            IsDeleted = false,
            AccountId = 32718250,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 999,
                    FieldName = "Должность",
                    Values = [new CustomFieldValue { Value = "Директор" }]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags = [new Tag { Id = 201, Name = "Новый" }],
                Companies = [new Company { Id = 66666 }]
            }
        };

        var json = JsonSerializer.Serialize(originalContact, _jsonOptions);

        // Act
        Contact? deserialized = JsonSerializer.Deserialize<Contact>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalContact.Id);
        deserialized.Name.Should().Be(originalContact.Name);
        deserialized.FirstName.Should().Be(originalContact.FirstName);
        deserialized.LastName.Should().Be(originalContact.LastName);
        deserialized.ResponsibleUserId.Should().Be(originalContact.ResponsibleUserId);
        deserialized.CreatedAt.Should().Be(originalContact.CreatedAt);
        deserialized.UpdatedAt.Should().Be(originalContact.UpdatedAt);
        deserialized.IsDeleted.Should().Be(originalContact.IsDeleted);
        deserialized.AccountId.Should().Be(originalContact.AccountId);

        // Проверка custom fields
        deserialized.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        var cf = deserialized.CustomFieldValues![0];
        cf.FieldId.Should().Be(999);
        cf.FieldName.Should().Be("Должность");
        cf.Values.Should().HaveCount(1);
        cf.Values[0].Value.Should().Be("Директор");

        // Embedded через базовый класс
        deserialized.Embedded.Should().NotBeNull();
        deserialized.Embedded!.Tags.Should().HaveCount(1);
        deserialized.Embedded.Tags![0].Name.Should().Be("Новый");
        deserialized.Embedded.Companies.Should().HaveCount(1);
        deserialized.Embedded.Companies![0].Id.Should().Be(66666);
    }

    [Fact]
    public void Should_Ignore_Contact_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99988877,
            "name": "Сидоров Сидор",
            "first_name": "Сидор",
            "last_name": "Сидоров",
            "responsible_user_id": 123456,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "is_deleted": false,
            "account_id": 32718250,
            "some_future_field": "ignore_me",
            "custom_fields_values": [
                {
                    "field_id": 1001,
                    "field_name": "Телефон",
                    "field_code": "PHONE",
                    "extra_custom_prop": "skip",
                    "values": [
                        {
                            "value": "+79001112233",
                            "unknown_value_prop": "skip",
                            "enum_id": 10
                        }
                    ]
                }
            ],
            "_embedded": {
                "tags": [
                    {
                        "id": 301,
                        "name": "Тест"
                    }
                ],
                "companies": []
            }
        }
        """;

        // Act
        Contact? contact = JsonSerializer.Deserialize<Contact>(jsonWithExtraFields, _jsonOptions);

        // Assert
        contact.Should().NotBeNull();
        contact!.Id.Should().Be(99988877);
        contact.Name.Should().Be("Сидоров Сидор");
        contact.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        CustomFieldValuesContainer cf = contact.CustomFieldValues![0];
        cf.FieldId.Should().Be(1001);
        cf.FieldName.Should().Be("Телефон");
        cf.Values.Should().HaveCount(1);
        cf.Values[0].Value.Should().Be("+79001112233");
        cf.Values[0].EnumId.Should().Be(10);

        contact.Embedded.Should().NotBeNull();
        contact.Embedded!.Tags.Should().HaveCount(1);
        contact.Embedded.Tags![0].Id.Should().Be(301);
        contact.Embedded.Tags[0].Name.Should().Be("Тест");
    }

    [Fact]
    public void Should_Handle_Contact_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 11122233,
            "name": null,
            "first_name": null,
            "last_name": "Безымянный",
            "responsible_user_id": null,
            "created_at": 1672531200,
            "updated_at": null,
            "is_deleted": false,
            "account_id": 32718250,
            "custom_fields_values": null,
            "_embedded": {
                "tags": null,
                "companies": []
            }
        }
        """;

        // Act
        Contact? contact = JsonSerializer.Deserialize<Contact>(jsonWithNulls, _jsonOptions);

        // Assert
        contact.Should().NotBeNull();
        contact!.Id.Should().Be(11122233);
        contact.Name.Should().BeNull();
        contact.FirstName.Should().BeNull();
        contact.LastName.Should().Be("Безымянный");
        contact.ResponsibleUserId.Should().BeNull();
        contact.CreatedAt.Should().Be(1672531200);
        contact.UpdatedAt.Should().BeNull();
        contact.IsDeleted.Should().BeFalse();
        contact.AccountId.Should().Be(32718250);
        contact.CustomFieldValues.Should().BeNull();

        contact.Embedded.Should().NotBeNull();
        contact.Embedded!.Tags.Should().BeNull();
        contact.Embedded.Companies.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public Task Should_Verify_Company_Response()
    {
        // Arrange
        var response = new Company
        {
            Id = 55566677,
            Name = "ООО Ромашка",
            ResponsibleUserId = 987654,
            ResponsibleUserGroupId = 222,
            CreatedBy = 987654,
            UpdatedBy = 987654,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            ClosestTaskAt = 1706745600,
            IsDeleted = false,
            AccountId = 32718250,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 11111,
                    FieldName = "ИНН",
                    FieldCode = "INN",
                    Values =
                    [
                        new CustomFieldValue { Value = "7701234567" }
                    ]
                },
                new CustomFieldValuesContainer
                {
                    FieldId = 22222,
                    FieldName = "Сфера деятельности",
                    FieldCode = "INDUSTRY",
                    Values =
                    [
                        new CustomFieldValue
                        {
                            Value = "IT",
                            EnumId = 501,
                            EnumCode = "IT"
                        }
                    ]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags =
                [
                    new Tag { Id = 201, Name = "Партнёр" },
                    new Tag { Id = 202, Name = "B2B" }
                ],
                Contacts =
                [
                    new Contact { Id = 12345678 }
                ]
            }
        };

        // Act & Assert
        return Verify(response)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Company_Response_From_Generated_Json()
    {
        // Arrange
        var originalCompany = new Company
        {
            Id = 55566677,
            Name = "ООО Ландыш",
            ResponsibleUserId = 111222,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            IsDeleted = false,
            AccountId = 32718250,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 33333,
                    FieldName = "Сайт",
                    Values = [new CustomFieldValue { Value = "https://example.com" }]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags = [new Tag { Id = 301, Name = "Клиент" }],
                Contacts = [new Contact { Id = 87654321 }]
            }
        };

        var json = JsonSerializer.Serialize(originalCompany, _jsonOptions);

        // Act
        Company? deserialized = JsonSerializer.Deserialize<Company>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalCompany.Id);
        deserialized.Name.Should().Be(originalCompany.Name);
        deserialized.ResponsibleUserId.Should().Be(originalCompany.ResponsibleUserId);
        deserialized.CreatedAt.Should().Be(originalCompany.CreatedAt);
        deserialized.UpdatedAt.Should().Be(originalCompany.UpdatedAt);
        deserialized.IsDeleted.Should().Be(originalCompany.IsDeleted);
        deserialized.AccountId.Should().Be(originalCompany.AccountId);

        // Custom fields
        deserialized.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        CustomFieldValuesContainer cf = deserialized.CustomFieldValues![0];
        cf.FieldId.Should().Be(33333);
        cf.FieldName.Should().Be("Сайт");
        cf.Values.Should().HaveCount(1);
        cf.Values[0].Value.Should().Be("https://example.com");

        // Embedded
        deserialized.Embedded.Should().NotBeNull();
        deserialized.Embedded!.Tags.Should().HaveCount(1);
        deserialized.Embedded.Tags![0].Name.Should().Be("Клиент");

        deserialized.Embedded.Contacts.Should().HaveCount(1);
        deserialized.Embedded.Contacts![0].Id.Should().Be(87654321);
    }

    [Fact]
    public void Should_Ignore_Company_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "ООО Неведомо",
            "responsible_user_id": 555666,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "is_deleted": false,
            "account_id": 32718250,
            "closest_task_at": 1706745600,
            "future_field": "ignore_me",
            "custom_fields_values": [
                {
                    "field_id": 44444,
                    "field_name": "Регион",
                    "field_code": "REGION",
                    "extra": "skip",
                    "values": [
                        {
                            "value": "Москва",
                            "unknown_prop": "skip"
                        }
                    ]
                }
            ],
            "_embedded": {
                "tags": [
                    {
                        "id": 401,
                        "name": "Тест",
                        "color": "#000000"
                    }
                ]
            }
        }
        """;

        // Act
        Company? company = JsonSerializer.Deserialize<Company>(jsonWithExtraFields, _jsonOptions);

        // Assert
        company.Should().NotBeNull();
        company!.Id.Should().Be(99900011);
        company.Name.Should().Be("ООО Неведомо");
        company.ResponsibleUserId.Should().Be(555666);
        company.CreatedAt.Should().Be(1672531200);
        company.UpdatedAt.Should().Be(1704067200);
        company.IsDeleted.Should().BeFalse();
        company.AccountId.Should().Be(32718250);

        company.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        company.CustomFieldValues![0].FieldName.Should().Be("Регион");
        company.CustomFieldValues[0].Values[0].Value.Should().Be("Москва");

        company.Embedded.Should().NotBeNull();
        company.Embedded!.Tags.Should().HaveCount(1);
        company.Embedded.Tags![0].Id.Should().Be(401);
        company.Embedded.Tags[0].Name.Should().Be("Тест");
    }

    [Fact]
    public void Should_Handle_Company_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": null,
            "responsible_user_id": null,
            "created_at": 1672531200,
            "updated_at": null,
            "is_deleted": false,
            "account_id": 32718250,
            "closest_task_at": null,
            "custom_fields_values": null,
            "_embedded": {
                "tags": null,
                "contacts": []
            }
        }
        """;

        // Act
        Company? company = JsonSerializer.Deserialize<Company>(jsonWithNulls, _jsonOptions);

        // Assert
        company.Should().NotBeNull();
        company!.Id.Should().Be(88899900);
        company.Name.Should().BeNull();
        company.ResponsibleUserId.Should().BeNull();
        company.CreatedAt.Should().Be(1672531200);
        company.UpdatedAt.Should().BeNull();
        company.ClosestTaskAt.Should().BeNull();
        company.IsDeleted.Should().BeFalse();
        company.AccountId.Should().Be(32718250);
        company.CustomFieldValues.Should().BeNull();

        company.Embedded.Should().NotBeNull();
        company.Embedded!.Tags.Should().BeNull();
        company.Embedded.Contacts.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public Task Should_Verify_Lead_Response()
    {
        // Arrange
        var lead = new Lead
        {
            Id = 3912171,
            Name = "Example",
            Price = 12,
            ResponsibleUserId = 504141,
            ResponsibleUserGroupId = 625,
            PipelineId = 3104455,
            StatusId = 143,
            LossReasonId = 4203748,
            CreatedBy = 504141,
            UpdatedBy = 504141,
            CreatedAt = 1585299171,
            UpdatedAt = 1590683337,
            ClosedAt = 1590683337,
            ClosestTaskAt = null,
            IsDeleted = false,
            AccountId = 28805383,
            Score = null,
            LaborCost = 1200,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 294471,
                    FieldName = "Комментарий",
                    Values =
                    [
                        new CustomFieldValue { Value = "Наш первый клиент" }
                    ]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags =
                [
                    new Tag { Id = 100667, Name = "тест", Color = "#ff0000" }
                ],
                Contacts =
                [
                    new Contact { Id = 10971465 }
                ],
                Companies =
                [
                    new Company { Id = 10971463 }
                ]
            }
        };

        // Act & Assert
        return Verify(lead)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Lead_Response_From_Generated_Json()
    {
        // Arrange
        var originalLead = new Lead
        {
            Id = 19619,
            Name = "Сделка для примера",
            Price = 46333,
            ResponsibleUserId = 123321,
            PipelineId = 1300,
            StatusId = 142,
            CreatedBy = 321123,
            UpdatedBy = 321123,
            CreatedAt = 1453279607,
            UpdatedAt = 1502193501,
            IsDeleted = false,
            AccountId = 5135160,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 99999,
                    FieldName = "Источник",
                    Values = [new CustomFieldValue { Value = "Сайт" }]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags = [new Tag { Id = 200, Name = "Важная", Color = "#00ff00" }],
                Contacts = [new Contact { Id = 88888 }],
                Companies = [new Company { Id = 77777 }]
            }
        };

        var json = JsonSerializer.Serialize(originalLead, _jsonOptions);

        // Act
        Lead? deserialized = JsonSerializer.Deserialize<Lead>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalLead.Id);
        deserialized.Name.Should().Be(originalLead.Name);
        deserialized.Price.Should().Be(originalLead.Price);
        deserialized.ResponsibleUserId.Should().Be(originalLead.ResponsibleUserId);
        deserialized.PipelineId.Should().Be(originalLead.PipelineId);
        deserialized.StatusId.Should().Be(originalLead.StatusId);
        deserialized.CreatedAt.Should().Be(originalLead.CreatedAt);
        deserialized.UpdatedAt.Should().Be(originalLead.UpdatedAt);
        deserialized.IsDeleted.Should().Be(originalLead.IsDeleted);
        deserialized.AccountId.Should().Be(originalLead.AccountId);

        deserialized.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        CustomFieldValuesContainer cf = deserialized.CustomFieldValues![0];
        cf.FieldName.Should().Be("Источник");
        cf.Values[0].Value.Should().Be("Сайт");

        deserialized.Embedded.Should().NotBeNull();
        deserialized.Embedded!.Tags.Should().HaveCount(1);
        deserialized.Embedded.Tags![0].Name.Should().Be("Важная");
        deserialized.Embedded.Tags[0].Color.Should().Be("#00ff00");

        deserialized.Embedded.Contacts.Should().HaveCount(1);
        deserialized.Embedded.Contacts![0].Id.Should().Be(88888);

        deserialized.Embedded.Companies.Should().HaveCount(1);
        deserialized.Embedded.Companies![0].Id.Should().Be(77777);
    }

    [Fact]
    public void Should_Ignore_Lead_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "Тестовая сделка",
            "price": 10000,
            "responsible_user_id": 555666,
            "pipeline_id": 1001,
            "status_id": 142,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "is_deleted": false,
            "account_id": 32718250,
            "future_field": "ignore_me",
            "custom_fields_values": [
                {
                    "field_id": 44444,
                    "field_name": "Приоритет",
                    "values": [
                        {
                            "value": "Высокий",
                            "unknown_prop": "skip"
                        }
                    ]
                }
            ],
            "_embedded": {
                "tags": [
                    {
                        "id": 401,
                        "name": "API",
                        "color": "#123456",
                        "extra_tag_prop": "skip"
                    }
                ]
            }
        }
        """;

        // Act
        Lead? lead = JsonSerializer.Deserialize<Lead>(jsonWithExtraFields, _jsonOptions);

        // Assert
        lead.Should().NotBeNull();
        lead!.Id.Should().Be(99900011);
        lead.Name.Should().Be("Тестовая сделка");
        lead.Price.Should().Be(10000);
        lead.ResponsibleUserId.Should().Be(555666);
        lead.PipelineId.Should().Be(1001);
        lead.StatusId.Should().Be(142);
        lead.IsDeleted.Should().BeFalse();
        lead.AccountId.Should().Be(32718250);

        lead.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        lead.CustomFieldValues![0].FieldName.Should().Be("Приоритет");
        lead.CustomFieldValues[0].Values[0].Value.Should().Be("Высокий");

        lead.Embedded.Should().NotBeNull();
        lead.Embedded!.Tags.Should().HaveCount(1);
        lead.Embedded.Tags![0].Id.Should().Be(401);
        lead.Embedded.Tags[0].Name.Should().Be("API");
        lead.Embedded.Tags[0].Color.Should().Be("#123456");
    }

    [Fact]
    public void Should_Handle_Lead_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": null,
            "price": 0,
            "responsible_user_id": null,
            "pipeline_id": 1001,
            "status_id": 142,
            "created_at": 1672531200,
            "updated_at": null,
            "closed_at": null,
            "is_deleted": false,
            "account_id": 32718250,
            "score": null,
            "labor_cost": null,
            "custom_fields_values": null,
            "_embedded": {
                "tags": null,
                "contacts": [],
                "companies": []
            }
        }
        """;

        // Act
        Lead? lead = JsonSerializer.Deserialize<Lead>(jsonWithNulls, _jsonOptions);

        // Assert
        lead.Should().NotBeNull();
        lead!.Id.Should().Be(88899900);
        lead.Name.Should().BeNull();
        lead.Price.Should().Be(0);
        lead.ResponsibleUserId.Should().BeNull();
        lead.PipelineId.Should().Be(1001);
        lead.StatusId.Should().Be(142);
        lead.CreatedAt.Should().Be(1672531200);
        lead.UpdatedAt.Should().BeNull();
        lead.ClosedAt.Should().BeNull();
        lead.IsDeleted.Should().BeFalse();
        lead.AccountId.Should().Be(32718250);
        lead.Score.Should().BeNull();
        lead.LaborCost.Should().BeNull();
        lead.CustomFieldValues.Should().BeNull();

        lead.Embedded.Should().NotBeNull();
        lead.Embedded!.Tags.Should().BeNull();
        lead.Embedded.Contacts.Should().NotBeNull().And.BeEmpty();
        lead.Embedded.Companies.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public Task Should_Verify_Customer_Response()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 134923,
            Name = "Иван Петров",
            NextPrice = 15000,
            NextDate = 1706745600,
            ResponsibleUserId = 504141,
            StatusId = 4740028,
            CreatedBy = 504141,
            UpdatedBy = 504141,
            CreatedAt = 1590943901,
            UpdatedAt = 1590943901,
            ClosestTaskAt = null,
            IsDeleted = false,
            TotalSpent = 1231454,
            PurchasesCount = 11,
            AverageCheck = 111950,
            AccountId = 28805383,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 294479,
                    FieldName = "Источник",
                    Values =
                    [
                        new CustomFieldValue { Value = "Сайт" }
                    ]
                },
                new CustomFieldValuesContainer
                {
                    FieldId = 999999,
                    FieldName = "VIP",
                    Values = [new CustomFieldValue { Value = "true" }]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags =
                [
                    new Tag { Id = 101, Name = "VIP" }
                ],
                Contacts =
                [
                    new Contact { Id = 9820781 }
                ],
                Companies =
                [
                    new Company { Id = 55555 }
                ]
            }
        };

        // Act & Assert
        return Verify(customer)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Customer_Response_From_Generated_Json()
    {
        // Arrange
        var originalCustomer = new Customer
        {
            Id = 1001,
            Name = "Алексей Смирнов",
            NextPrice = 5000,
            NextDate = 1704067200,
            ResponsibleUserId = 123456,
            StatusId = 4740010,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            IsDeleted = false,
            TotalSpent = 25000,
            PurchasesCount = 3,
            AverageCheck = 8333,
            AccountId = 32718250,
            CustomFieldValues =
            [
                new CustomFieldValuesContainer
                {
                    FieldId = 99999,
                    FieldName = "Категория",
                    Values = [new CustomFieldValue { Value = "Постоянный" }]
                },
                new CustomFieldValuesContainer
                {
                    FieldId = 999999,
                    FieldName = "VIP",
                    Values = [new CustomFieldValue { Value = "true" }]
                }
            ],
            Embedded = new EmbeddedEntitiesResponse
            {
                Tags = [new Tag { Id = 201, Name = "Клиент" }],
                Contacts = [new Contact { Id = 88888 }],
                Companies = [new Company { Id = 77777 }]
            }
        };

        var json = JsonSerializer.Serialize(originalCustomer, _jsonOptions);

        // Act
        Customer? deserialized = JsonSerializer.Deserialize<Customer>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalCustomer.Id);
        deserialized.Name.Should().Be(originalCustomer.Name);
        deserialized.NextPrice.Should().Be(originalCustomer.NextPrice);
        deserialized.NextDate.Should().Be(originalCustomer.NextDate);
        deserialized.ResponsibleUserId.Should().Be(originalCustomer.ResponsibleUserId);
        deserialized.StatusId.Should().Be(originalCustomer.StatusId);
        deserialized.CreatedAt.Should().Be(originalCustomer.CreatedAt);
        deserialized.UpdatedAt.Should().Be(originalCustomer.UpdatedAt);
        deserialized.IsDeleted.Should().Be(originalCustomer.IsDeleted);
        deserialized.TotalSpent.Should().Be(originalCustomer.TotalSpent);
        deserialized.PurchasesCount.Should().Be(originalCustomer.PurchasesCount);
        deserialized.AverageCheck.Should().Be(originalCustomer.AverageCheck);
        deserialized.AccountId.Should().Be(originalCustomer.AccountId);

        deserialized.CustomFieldValues.Should().NotBeNull().And.HaveCount(2);
        CustomFieldValuesContainer firstField = deserialized.CustomFieldValues![0];
        firstField.FieldName.Should().Be("Категория");
        firstField.Values[0].Value.Should().Be("Постоянный");
        CustomFieldValuesContainer secondField = deserialized.CustomFieldValues![1];
        secondField.FieldName.Should().Be("VIP");
        secondField.Values[0].Value.Should().Be("true");

        // Embedded
        deserialized.Embedded.Should().NotBeNull();
        deserialized.Embedded!.Tags.Should().HaveCount(1);
        deserialized.Embedded.Tags![0].Name.Should().Be("Клиент");

        deserialized.Embedded.Contacts.Should().HaveCount(1);
        deserialized.Embedded.Contacts![0].Id.Should().Be(88888);

        deserialized.Embedded.Companies.Should().HaveCount(1);
        deserialized.Embedded.Companies![0].Id.Should().Be(77777);
    }

    [Fact]
    public void Should_Ignore_Customer_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "Новый покупатель",
            "next_price": 10000,
            "next_date": 1706745600,
            "responsible_user_id": 555666,
            "status_id": 4740028,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "is_deleted": false,
            "ltv": 0,
            "purchases_count": 0,
            "average_check": 0,
            "account_id": 32718250,
            "future_field": "ignore_me",
            "custom_fields_values": [
                {
                    "field_id": 44444,
                    "field_name": "Приоритет",
                    "values": [
                        {
                            "value": "Высокий",
                            "unknown_prop": "skip"
                        }
                    ]
                }
            ],
            "_embedded": {
                "tags": [
                    {
                        "id": 301,
                        "name": "Тест",
                        "color": "#123456"
                    }
                ],
                "contacts": [
                    {
                        "id": 11223344
                    }
                ],
                "companies": [
                    {
                        "id": 55667788
                    }
                ]
            }
        }
        """;

        // Act
        Customer? customer = JsonSerializer.Deserialize<Customer>(jsonWithExtraFields, _jsonOptions);

        // Assert
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(99900011);
        customer.Name.Should().Be("Новый покупатель");
        customer.NextPrice.Should().Be(10000);
        customer.NextDate.Should().Be(1706745600);
        customer.ResponsibleUserId.Should().Be(555666);
        customer.StatusId.Should().Be(4740028);
        customer.IsDeleted.Should().BeFalse();
        customer.AccountId.Should().Be(32718250);

        customer.CustomFieldValues.Should().NotBeNull().And.HaveCount(1);
        customer.CustomFieldValues![0].FieldName.Should().Be("Приоритет");
        customer.CustomFieldValues[0].Values[0].Value.Should().Be("Высокий");

        customer.Embedded.Should().NotBeNull();
        customer.Embedded!.Tags.Should().HaveCount(1);
        customer.Embedded.Tags![0].Id.Should().Be(301);
        customer.Embedded.Tags[0].Name.Should().Be("Тест");

        customer.Embedded.Contacts.Should().HaveCount(1);
        customer.Embedded.Contacts![0].Id.Should().Be(11223344);

        customer.Embedded.Companies.Should().HaveCount(1);
        customer.Embedded.Companies![0].Id.Should().Be(55667788);
    }

    [Fact]
    public void Should_Handle_Customer_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": "Безымянный покупатель",
            "next_price": null,
            "next_date": null,
            "responsible_user_id": null,
            "status_id": null,
            "created_at": 1672531200,
            "updated_at": null,
            "closest_task_at": null,
            "is_deleted": false,
            "ltv": null,
            "purchases_count": null,
            "average_check": null,
            "account_id": 32718250,
            "custom_fields_values": null,
            "_embedded": {
                "tags": [],
                "contacts": [],
                "companies": []
            }
        }
        """;

        // Act
        Customer? customer = JsonSerializer.Deserialize<Customer>(jsonWithNulls, _jsonOptions);

        // Assert
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(88899900);
        customer.Name.Should().Be("Безымянный покупатель");
        customer.NextPrice.Should().BeNull();
        customer.NextDate.Should().BeNull();
        customer.ResponsibleUserId.Should().BeNull();
        customer.StatusId.Should().BeNull();
        customer.CreatedAt.Should().Be(1672531200);
        customer.UpdatedAt.Should().BeNull();
        customer.ClosestTaskAt.Should().BeNull();
        customer.IsDeleted.Should().BeFalse();
        customer.TotalSpent.Should().BeNull();
        customer.PurchasesCount.Should().BeNull();
        customer.AverageCheck.Should().BeNull();
        customer.AccountId.Should().Be(32718250);
        customer.CustomFieldValues.Should().BeNull();

        customer.Embedded.Should().NotBeNull();
        customer.Embedded!.Tags.Should().NotBeNull().And.BeEmpty();
        customer.Embedded.Contacts.Should().NotBeNull().And.BeEmpty();
        customer.Embedded.Companies.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public Task Should_Verify_Task_Response()
    {
        // Arrange
        var task = new AmoCrmTask
        {
            Id = 7087,
            ResponsibleUserId = 123123,
            ResponsibleUserGroupId = 0,
            CreatedBy = 3987910,
            UpdatedBy = 3987910,
            CreatedAt = 1575364000,
            UpdatedAt = 1575364851,
            EntityId = 167353,
            EntityType = EntityType.Leads,
            IsCompleted = true,
            TaskTypeId = 2,
            Description = "Пригласить на бесплатное обучение",
            Duration = 0,
            CompleteTill = 1575665940,
            AccountId = 321321,
            Result = new TaskResult { Text = "Клиент согласился" }
        };

        // Act & Assert
        return Verify(task)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Task_Response_From_Generated_Json()
    {
        // Arrange
        var originalTask = new AmoCrmTask
        {
            Id = 215089,
            ResponsibleUserId = 123123,
            CreatedBy = 0,
            UpdatedBy = 3987910,
            CreatedAt = 1576767879,
            UpdatedAt = 1576767914,
            EntityId = 1035487,
            EntityType = EntityType.Leads,
            IsCompleted = true,
            TaskTypeId = 2,
            Description = "Назначить встречу с клиентом",
            CompleteTill = 1576768179,
            AccountId = 321312,
            Result = new TaskResult { Text = "Клиент согласился" }
        };

        var json = JsonSerializer.Serialize(originalTask, _jsonOptions);

        // Act
        AmoCrmTask? deserialized = JsonSerializer.Deserialize<AmoCrmTask>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalTask.Id);
        deserialized.ResponsibleUserId.Should().Be(originalTask.ResponsibleUserId);
        deserialized.CreatedBy.Should().Be(originalTask.CreatedBy);
        deserialized.UpdatedAt.Should().Be(originalTask.UpdatedAt);
        deserialized.EntityId.Should().Be(originalTask.EntityId);
        deserialized.EntityType.Should().Be(originalTask.EntityType);
        deserialized.IsCompleted.Should().Be(originalTask.IsCompleted);
        deserialized.TaskTypeId.Should().Be(originalTask.TaskTypeId);
        deserialized.Description.Should().Be(originalTask.Description);
        deserialized.CompleteTill.Should().Be(originalTask.CompleteTill);
        deserialized.AccountId.Should().Be(originalTask.AccountId);
        deserialized.Result.Should().NotBeNull();
        deserialized.Result!.Text.Should().Be(originalTask.Result.Text);
    }

    [Fact]
    public void Should_Ignore_Task_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "responsible_user_id": 555666,
            "group_id": 100,
            "created_by": 111222,
            "updated_by": 111222,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "entity_id": 88888,
            "entity_type": "contacts",
            "is_completed": false,
            "task_type_id": 1,
            "text": "Позвонить клиенту",
            "duration": 300,
            "complete_till": 1706745600,
            "account_id": 32718250,
            "result": {
                "text": "Дозвонился"
            },
            "future_field": "ignore_me",
            "extra_array": [1, 2, 3]
        }
        """;

        // Act
        AmoCrmTask? task = JsonSerializer.Deserialize<AmoCrmTask>(jsonWithExtraFields, _jsonOptions);

        // Assert
        task.Should().NotBeNull();
        task!.Id.Should().Be(99900011);
        task.ResponsibleUserId.Should().Be(555666);
        task.EntityId.Should().Be(88888);
        task.EntityType.Should().Be(EntityType.Contacts);
        task.IsCompleted.Should().BeFalse();
        task.TaskTypeId.Should().Be(1);
        task.Description.Should().Be("Позвонить клиенту");
        task.Duration.Should().Be(300);
        task.CompleteTill.Should().Be(1706745600);
        task.AccountId.Should().Be(32718250);
        task.Result.Should().NotBeNull();
        task.Result!.Text.Should().Be("Дозвонился");
    }

    [Fact]
    public void Should_Handle_Task_Response_Json_With_Null_Values()
    {
        // Arrange
        // amoCRM возвращает пустые результаты в виде массива, а не null. Поэтому result: []
        var jsonWithNulls = """
        {
            "id": 88899900,
            "responsible_user_id": null,
            "group_id": null,
            "created_by": null,
            "updated_by": null,
            "created_at": null,
            "updated_at": 1672531200,
            "entity_id": null,
            "entity_type": null,
            "is_completed": false,
            "task_type_id": null,
            "text": "Задача без привязки",
            "duration": null,
            "complete_till": 1706745600,
            "account_id": 32718250,
            "result": []
        }
        """;

        // Act
        AmoCrmTask? task = JsonSerializer.Deserialize<AmoCrmTask>(jsonWithNulls, _jsonOptions);

        // Assert
        task.Should().NotBeNull();
        task!.Id.Should().Be(88899900);
        task.ResponsibleUserId.Should().BeNull();
        task.ResponsibleUserGroupId.Should().BeNull();
        task.CreatedBy.Should().BeNull();
        task.UpdatedBy.Should().BeNull();
        task.CreatedAt.Should().BeNull();
        task.UpdatedAt.Should().Be(1672531200);
        task.EntityId.Should().BeNull();
        task.EntityType.Should().BeNull();
        task.IsCompleted.Should().BeFalse();
        task.TaskTypeId.Should().BeNull();
        task.Description.Should().Be("Задача без привязки");
        task.Duration.Should().BeNull();
        task.CompleteTill.Should().Be(1706745600);
        task.AccountId.Should().Be(32718250);
        task.Result.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_EntityLink_Response()
    {
        // Arrange
        var link = new EntityLink
        {
            EntityId = 7593303,
            EntityType = EntityType.Leads,
            LinkedEntityId = 11069775,
            LinkedEntityType = EntityType.Contacts,
            Metadata = new LinkedEntityMetadata
            {
                IsMainContact = true
            }
        };

        // Act & Assert
        return Verify(link)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_EntityLink_Response_From_Generated_Json()
    {
        // Arrange
        var originalLink = new EntityLink
        {
            EntityId = 14158851,
            EntityType = EntityType.Leads,
            LinkedEntityId = 10,
            LinkedEntityType = EntityType.Contacts,
            Metadata = new LinkedEntityMetadata
            {
                IsMainContact = true,
                PriceCustomFieldId = 98765
            }
        };

        var json = JsonSerializer.Serialize(originalLink, _jsonOptions);

        // Act
        EntityLink? deserialized = JsonSerializer.Deserialize<EntityLink>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.EntityId.Should().Be(originalLink.EntityId);
        deserialized.EntityType.Should().Be(originalLink.EntityType);
        deserialized.LinkedEntityId.Should().Be(originalLink.LinkedEntityId);
        deserialized.LinkedEntityType.Should().Be(originalLink.LinkedEntityType);

        deserialized.Metadata.Should().NotBeNull();
        deserialized.Metadata!.PriceCustomFieldId.Should().Be(98765);
        deserialized.Metadata.IsMainContact.Should().BeTrue();
    }

    [Fact]
    public void Should_Ignore_EntityLink_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "entity_id": 99900011,
            "entity_type": "contacts",
            "to_entity_id": 88800022,
            "to_entity_type": "companies",
            "metadata": {
                "is_main": true,
                "updated_by": 123456,
                "future_metadata_prop": "ignore_me"
            },
            "extra_top_level_field": "skip"
        }
        """;

        // Act
        EntityLink? link = JsonSerializer.Deserialize<EntityLink>(jsonWithExtraFields, _jsonOptions);

        // Assert
        link.Should().NotBeNull();
        link!.EntityId.Should().Be(99900011);
        link.EntityType.Should().Be(EntityType.Contacts);
        link.LinkedEntityId.Should().Be(88800022);
        link.LinkedEntityType.Should().Be(EntityType.Companies);

        link.Metadata.Should().NotBeNull();
        link.Metadata!.IsMainContact.Should().BeTrue();
        link.Metadata.UpdatedBy.Should().Be(123456);
    }

    [Fact]
    public void Should_Handle_EntityLink_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "entity_id": 11122233,
            "entity_type": "customers",
            "to_entity_id": 44455566,
            "to_entity_type": "leads",
            "metadata": {
                "is_main": null,
                "updated_by": 123456,
                "catalog_id": null,
                "quantity": null,
                "price_id": null
            }
        }
        """;

        // Act
        EntityLink? link = JsonSerializer.Deserialize<EntityLink>(jsonWithNulls, _jsonOptions);

        // Assert
        link.Should().NotBeNull();
        link!.EntityId.Should().Be(11122233);
        link.EntityType.Should().Be(EntityType.Customers);
        link.LinkedEntityId.Should().Be(44455566);
        link.LinkedEntityType.Should().Be(EntityType.Leads);
        link.Metadata.Should().NotBeNull();
        link.Metadata!.UpdatedBy.Should().Be(123456);
        link.Metadata.CatalogId.Should().BeNull();
        link.Metadata.CatalogElementsCount.Should().BeNull();
        link.Metadata.IsMainContact.Should().BeNull();
        link.Metadata.PriceCustomFieldId.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_Common_Note_Response()
    {
        var note = new Note
        {
            Id = 42709325,
            EntityId = 26050861,
            ResponsibleUserId = 939801,
            CreatedBy = 940088,
            UpdatedBy = 940088,
            CreatedAt = 1540407495,
            UpdatedAt = 1540408317,
            NoteType = NoteType.Common,
            AccountId = 17079858,
            Parameters = new NoteDetails
            {
                Text = "Текст примечания"
            }
        };

        return Verify(note)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public Task Should_Verify_CallIn_Note_Response()
    {
        var note = new Note
        {
            Id = 76787983,
            EntityId = 167353,
            ResponsibleUserId = 8238874,
            CreatedBy = 8238874,
            UpdatedBy = 8238874,
            CreatedAt = 1588841241,
            UpdatedAt = 1588841241,
            NoteType = NoteType.CallIn,
            AccountId = 17079858,
            Parameters = new NoteDetails
            {
                Text = "Входящий звонок",
                Uuid = "8f52d38a-5fb3-406d-93a3-a4832dc28f8b",
                Duration = 60,
                Source = "onlinePBX",
                Phone = "+79999999999",
                CallResponsibleUserId = 8238874
            }
        };

        return Verify(note)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public Task Should_Verify_GeoLocation_Note_Response()
    {
        var note = new Note
        {
            Id = 76787987,
            EntityId = 167353,
            ResponsibleUserId = 8238874,
            CreatedBy = 8238874,
            UpdatedAt = 1588841241,
            NoteType = NoteType.Geolocation,
            AccountId = 17079858,
            Parameters = new NoteDetails
            {
                Text = "Примечание с геолокацией",
                Address = "ул. Пушкина, дом Колотушкина",
                Longitude = 53.714816f,
                Latitude = 91.423146f
            }
        };

        return Verify(note)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public Task Should_Verify_Attachment_Note_Response()
    {
        var note = new Note
        {
            Id = 42709842,
            EntityId = 26053794,
            ResponsibleUserId = 939801,
            CreatedBy = 939801,
            UpdatedAt = 1548280115,
            NoteType = NoteType.Attachment,
            AccountId = 17079858,
            Parameters = new NoteDetails
            {
                Text = "Снимок экрана 2022-12-12 в 20.11.45 (1).jpg",
                FileId = Guid.Parse("6905db7c-3a29-4d30-8953-bac68c05e8e8"),
                FileVersionId = Guid.Parse("4e316440-4122-4cad-b121-9709882b4cc1"),
                FileName = "Snimok_ekrana_2022-12-12_v_20.11.45_1_.jpg"
            }
        };

        return Verify(note)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Note_Response_From_Generated_Json()
    {
        // Arrange
        var originalNote = new Note
        {
            Id = 12345,
            EntityId = 98765,
            ResponsibleUserId = 111222,
            CreatedBy = 111222,
            UpdatedBy = 111222,
            CreatedAt = 1672531200,
            UpdatedAt = 1704067200,
            NoteType = NoteType.SmsOut,
            AccountId = 32718250,
            Parameters = new NoteDetails
            {
                Text = "Новое исходящее SMS",
                Phone = "+79001112233"
            }
        };

        var json = JsonSerializer.Serialize(originalNote, _jsonOptions);

        // Act
        Note? deserialized = JsonSerializer.Deserialize<Note>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalNote.Id);
        deserialized.EntityId.Should().Be(originalNote.EntityId);
        deserialized.ResponsibleUserId.Should().Be(originalNote.ResponsibleUserId);
        deserialized.CreatedAt.Should().Be(originalNote.CreatedAt);
        deserialized.UpdatedAt.Should().Be(originalNote.UpdatedAt);
        deserialized.NoteType.Should().Be(NoteType.SmsOut);
        deserialized.AccountId.Should().Be(originalNote.AccountId);

        deserialized.Parameters.Should().NotBeNull();
        deserialized.Parameters!.Text.Should().Be("Новое исходящее SMS");
        deserialized.Parameters.Phone.Should().Be("+79001112233");
        deserialized.Parameters.Duration.Should().BeNull();
    }

    [Fact]
    public void Should_Ignore_Note_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "entity_id": 88800022,
            "responsible_user_id": 555666,
            "created_by": 111222,
            "updated_by": 111222,
            "created_at": 1672531200,
            "updated_at": 1704067200,
            "note_type": "call_out",
            "account_id": 32718250,
            "params": {
                "text": "Исходящий звонок",
                "phone": "+79991234567",
                "duration": 120,
                "source": "api",
                "call_responsible": 555666,
                "extra_param": "ignore_me",
                "unknown_field": true
            },
            "future_top_level_field": "skip"
        }
        """;

        // Act
        Note? note = JsonSerializer.Deserialize<Note>(jsonWithExtraFields, _jsonOptions);

        // Assert
        note.Should().NotBeNull();
        note!.Id.Should().Be(99900011);
        note.EntityId.Should().Be(88800022);
        note.NoteType.Should().Be(NoteType.CallOut);
        note.AccountId.Should().Be(32718250);

        note.Parameters.Should().NotBeNull();
        note.Parameters!.Text.Should().Be("Исходящий звонок");
        note.Parameters.Phone.Should().Be("+79991234567");
        note.Parameters.Duration.Should().Be(120);
        note.Parameters.Source.Should().Be("api");
        note.Parameters.CallResponsibleUserId.Should().Be(555666);
    }

    [Fact]
    public void Should_Handle_Note_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "entity_id": 77788899,
            "responsible_user_id": null,
            "group_id": null,
            "created_by": null,
            "updated_by": null,
            "created_at": null,
            "updated_at": 1672531200,
            "note_type": "common",
            "account_id": 32718250,
            "params": {
                "text": "Примечание без деталей",
                "phone": null,
                "duration": null,
                "address": null
            }
        }
        """;

        // Act
        Note? note = JsonSerializer.Deserialize<Note>(jsonWithNulls, _jsonOptions);

        // Assert
        note.Should().NotBeNull();
        note!.Id.Should().Be(88899900);
        note.EntityId.Should().Be(77788899);
        note.ResponsibleUserId.Should().BeNull();
        note.CreatedBy.Should().BeNull();
        note.CreatedAt.Should().BeNull();
        note.UpdatedAt.Should().Be(1672531200);
        note.NoteType.Should().Be(NoteType.Common);
        note.AccountId.Should().Be(32718250);

        note.Parameters.Should().NotBeNull();
        note.Parameters!.Text.Should().Be("Примечание без деталей");
        note.Parameters.Phone.Should().BeNull();
        note.Parameters.Duration.Should().BeNull();
        note.Parameters.Address.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_Transaction_Response()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = 134643,
            Comment = null,
            Price = 123,
            CompletedAt = 1591005900,
            CustomerId = 1000000158,
            CreatedBy = 939801,
            UpdatedBy = 939801,
            CreatedAt = 1591005900,
            UpdatedAt = 1591005900,
            IsDeleted = false,
            AccountId = 17079858
        };

        // Act & Assert
        return Verify(transaction)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Transaction_Response_From_Generated_Json()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            Id = 14755,
            Comment = "Транзакция",
            Price = 123124,
            CompletedAt = 1589025179,
            CustomerId = 1,
            CreatedBy = 504141,
            UpdatedBy = 504141,
            CreatedAt = 1589025179,
            UpdatedAt = 1589025179,
            IsDeleted = false,
            AccountId = 28805383
        };

        var json = JsonSerializer.Serialize(originalTransaction, _jsonOptions);

        // Act
        Transaction? deserialized = JsonSerializer.Deserialize<Transaction>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalTransaction.Id);
        deserialized.Comment.Should().Be(originalTransaction.Comment);
        deserialized.Price.Should().Be(originalTransaction.Price);
        deserialized.CompletedAt.Should().Be(originalTransaction.CompletedAt);
        deserialized.CustomerId.Should().Be(originalTransaction.CustomerId);
        deserialized.CreatedBy.Should().Be(originalTransaction.CreatedBy);
        deserialized.UpdatedAt.Should().Be(originalTransaction.UpdatedAt);
        deserialized.IsDeleted.Should().Be(originalTransaction.IsDeleted);
        deserialized.AccountId.Should().Be(originalTransaction.AccountId);
    }

    [Fact]
    public void Should_Ignore_Transaction_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "comment": "Покупка через сайт",
            "price": 5000,
            "completed_at": 1706745600,
            "customer_id": 88888,
            "created_by": 111222,
            "updated_by": 111222,
            "created_at": 1706745600,
            "updated_at": 1706745600,
            "is_deleted": false,
            "account_id": 32718250,
            "future_field": "ignore_me"
        }
        """;

        // Act
        Transaction? transaction = JsonSerializer.Deserialize<Transaction>(jsonWithExtraFields, _jsonOptions);

        // Assert
        transaction.Should().NotBeNull();
        transaction!.Id.Should().Be(99900011);
        transaction.Comment.Should().Be("Покупка через сайт");
        transaction.Price.Should().Be(5000);
        transaction.CustomerId.Should().Be(88888);
        transaction.AccountId.Should().Be(32718250);
    }

    [Fact]
    public void Should_Handle_Transaction_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "comment": null,
            "price": 0,
            "completed_at": null,
            "customer_id": 77788899,
            "created_by": null,
            "updated_by": null,
            "created_at": null,
            "updated_at": 1672531200,
            "is_deleted": false,
            "account_id": 32718250
        }
        """;

        // Act
        Transaction? transaction = JsonSerializer.Deserialize<Transaction>(jsonWithNulls, _jsonOptions);

        // Assert
        transaction.Should().NotBeNull();
        transaction!.Id.Should().Be(88899900);
        transaction.Comment.Should().BeNull();
        transaction.Price.Should().Be(0);
        transaction.CompletedAt.Should().BeNull();
        transaction.CustomerId.Should().Be(77788899);
        transaction.CreatedBy.Should().BeNull();
        transaction.UpdatedAt.Should().Be(1672531200);
        transaction.IsDeleted.Should().BeFalse();
        transaction.AccountId.Should().Be(32718250);
    }

    [Fact]
    public Task Should_Verify_Account_Response()
    {
        // Arrange
        var account = new AccountResponse
        {
            Id = 28805383,
            Name = "Example Account",
            Subdomain = "example",
            CurrentUserId = 504141,
            Country = "RU",
            CustomersMode = CustomersMode.Segments,
            IsLossReasonTrackingEnabled = true,
            IsUnsortedEnabled = true,
            IsHelperBotEnabled = false,
            IsTechnicalAccount = false
        };

        // Act & Assert
        return Verify(account)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Account_Response_From_Generated_Json()
    {
        // Arrange
        var originalAccount = new AccountResponse
        {
            Id = 12345678,
            Name = "My Company CRM",
            Subdomain = "mycompany",
            CurrentUserId = 987654,
            Country = "US",
            CustomersMode = CustomersMode.Periodicity,
            IsLossReasonTrackingEnabled = false,
            IsUnsortedEnabled = true,
            IsHelperBotEnabled = true,
            IsTechnicalAccount = false
        };

        var json = JsonSerializer.Serialize(originalAccount, _jsonOptions);

        // Act
        AccountResponse? deserialized = JsonSerializer.Deserialize<AccountResponse>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalAccount.Id);
        deserialized.Name.Should().Be(originalAccount.Name);
        deserialized.Subdomain.Should().Be(originalAccount.Subdomain);
        deserialized.CurrentUserId.Should().Be(originalAccount.CurrentUserId);
        deserialized.Country.Should().Be(originalAccount.Country);
        deserialized.CustomersMode.Should().Be(originalAccount.CustomersMode);
        deserialized.IsLossReasonTrackingEnabled.Should().Be(originalAccount.IsLossReasonTrackingEnabled);
        deserialized.IsUnsortedEnabled.Should().Be(originalAccount.IsUnsortedEnabled);
        deserialized.IsHelperBotEnabled.Should().Be(originalAccount.IsHelperBotEnabled);
        deserialized.IsTechnicalAccount.Should().Be(originalAccount.IsTechnicalAccount);
    }

    [Fact]
    public void Should_Ignore_Account_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "Test Account",
            "subdomain": "test",
            "current_user_id": 111222,
            "country": "DE",
            "customers_mode": "segments",
            "is_loss_reason_enabled": true,
            "is_unsorted_on": false,
            "is_helpbot_enabled": true,
            "is_technical_account": false,
            "future_field": "ignore_me",
            "extra_array": [1, 2, 3],
            "some_nested": {
                "prop": "skip"
            }
        }
        """;

        // Act
        AccountResponse? account = JsonSerializer.Deserialize<AccountResponse>(jsonWithExtraFields, _jsonOptions);

        // Assert
        account.Should().NotBeNull();
        account!.Id.Should().Be(99900011);
        account.Name.Should().Be("Test Account");
        account.Subdomain.Should().Be("test");
        account.CurrentUserId.Should().Be(111222);
        account.Country.Should().Be("DE");
        account.CustomersMode.Should().Be(CustomersMode.Segments);
        account.IsLossReasonTrackingEnabled.Should().BeTrue();
        account.IsUnsortedEnabled.Should().BeFalse();
        account.IsHelperBotEnabled.Should().BeTrue();
        account.IsTechnicalAccount.Should().BeFalse();
    }

    [Fact]
    public void Should_Handle_Account_Response_Json_With_Null_Values()
    {
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": null,
            "subdomain": null,
            "current_user_id": 555666,
            "country": null,
            "customers_mode": "periodicity",
            "is_loss_reason_enabled": false,
            "is_unsorted_on": true,
            "is_helpbot_enabled": false,
            "is_technical_account": true
        }
        """;

        // Act
        AccountResponse? account = JsonSerializer.Deserialize<AccountResponse>(jsonWithNulls, _jsonOptions);

        // Assert
        account.Should().NotBeNull();
        account!.Id.Should().Be(88899900);
        account.Name.Should().BeNull();
        account.Subdomain.Should().BeNull();
        account.CurrentUserId.Should().Be(555666);
        account.Country.Should().BeNull();
        account.CustomersMode.Should().Be(CustomersMode.Periodicity);
        account.IsLossReasonTrackingEnabled.Should().BeFalse();
        account.IsUnsortedEnabled.Should().BeTrue();
        account.IsHelperBotEnabled.Should().BeFalse();
        account.IsTechnicalAccount.Should().BeTrue();
    }

    [Fact]
    public Task Should_Verify_Widget_Response()
    {
        // Arrange
        var widget = new WidgetResponse
        {
            Id = 12345,
            Code = "my_widget",
            Version = "1.0.0",
            Rating = 4.5f,
            IsLeadSource = true,
            IsDigitalPipelineAvailable = false,
            IsCrmTemplate = false,
            ClientUuid = Guid.Parse("a1b2c3d4-e5f6-7890-a1b2-a3c4d5e6f7b8"),
            IsActiveInAccount = true,
            PipelineId = 3104455,
            SettingsTemplates =
            [
                new WidgetSettingsTemplate
                {
                    Key = "api_key",
                    Type = WidgetSettingFieldType.Text,
                    Name = "API ключ",
                    IsRequired = true
                },
                new WidgetSettingsTemplate
                {
                    Key = "enable_notifications",
                    Type = WidgetSettingFieldType.Custom,
                    Name = "Включить уведомления",
                    IsRequired = false
                }
            ],
            WidgetSettings = "{\"api_key\":\"secret123\",\"enable_notifications\":true}"
        };

        // Act & Assert
        return Verify(widget)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_Widget_Response_From_Generated_Json()
    {
        // Arrange
        var originalWidget = new WidgetResponse
        {
            Id = 67890,
            Code = "example_widget",
            Version = "2.1.5",
            Rating = 3.8f,
            IsLeadSource = false,
            IsDigitalPipelineAvailable = true,
            IsCrmTemplate = true,
            ClientUuid = null,
            IsActiveInAccount = false,
            PipelineId = 0,
            SettingsTemplates =
            [
                new WidgetSettingsTemplate
                {
                    Key = "api_key",
                    Type = WidgetSettingFieldType.Text,
                    Name = "API ключ",
                    IsRequired = true
                },
                new WidgetSettingsTemplate
                {
                    Key = "enable_notifications",
                    Type = WidgetSettingFieldType.Custom,
                    Name = "Включить уведомления",
                    IsRequired = false
                }
            ],
            WidgetSettings = null
        };

        var json = JsonSerializer.Serialize(originalWidget, _jsonOptions);

        // Act
        WidgetResponse? deserialized = JsonSerializer.Deserialize<WidgetResponse>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalWidget.Id);
        deserialized.Code.Should().Be(originalWidget.Code);
        deserialized.Version.Should().Be(originalWidget.Version);
        deserialized.Rating.Should().Be(originalWidget.Rating);
        deserialized.IsLeadSource.Should().Be(originalWidget.IsLeadSource);
        deserialized.IsDigitalPipelineAvailable.Should().Be(originalWidget.IsDigitalPipelineAvailable);
        deserialized.IsCrmTemplate.Should().Be(originalWidget.IsCrmTemplate);
        deserialized.ClientUuid.Should().BeNull();
        deserialized.IsActiveInAccount.Should().BeFalse();
        deserialized.PipelineId.Should().Be(0);
        deserialized.SettingsTemplates.Should().NotBeEmpty();

        WidgetSettingsTemplate apiKeyTemplate = deserialized.SettingsTemplates[0];
        apiKeyTemplate.Key.Should().Be("api_key");
        apiKeyTemplate.Type.Should().Be(WidgetSettingFieldType.Text);
        apiKeyTemplate.Name.Should().Be("API ключ");
        apiKeyTemplate.IsRequired.Should().BeTrue();

        WidgetSettingsTemplate notificationsTemplate = deserialized.SettingsTemplates[1];
        notificationsTemplate.Key.Should().Be("enable_notifications");
        notificationsTemplate.Type.Should().Be(WidgetSettingFieldType.Custom);
        notificationsTemplate.Name.Should().Be("Включить уведомления");
        notificationsTemplate.IsRequired.Should().BeFalse();
    }

    [Fact]
    public void Should_Ignore_Widget_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "code": "test_widget",
            "version": "1.2.3",
            "rating": 5.0,
            "is_lead_source": true,
            "is_work_with_dp": false,
            "is_crm_template": false,
            "client_uuid": "a1b2c3d4-e5f6-7890-a1b2-a3c4d5e6f7b8",
            "is_active_in_account": true,
            "pipeline_id": 1001,
            "settings_template": [],
            "settings": "{\"debug\":true}",
            "future_field": "ignore_me",
            "extra_array": [1, 2, 3]
        }
        """;

        // Act
        WidgetResponse? widget = JsonSerializer.Deserialize<WidgetResponse>(jsonWithExtraFields, _jsonOptions);

        // Assert
        widget.Should().NotBeNull();
        widget!.Id.Should().Be(99900011);
        widget.Code.Should().Be("test_widget");
        widget.Version.Should().Be("1.2.3");
        widget.Rating.Should().Be(5.0f);
        widget.IsLeadSource.Should().BeTrue();
        widget.IsDigitalPipelineAvailable.Should().BeFalse();
        widget.IsCrmTemplate.Should().BeFalse();
        widget.ClientUuid.Should().NotBeNull();
        widget.IsActiveInAccount.Should().BeTrue();
        widget.PipelineId.Should().Be(1001);
        widget.SettingsTemplates.Should().BeEmpty();
        widget.WidgetSettings.Should().Be("{\"debug\":true}");
    }

    [Fact]
    public void Should_Handle_Widget_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "code": null,
            "version": null,
            "rating": 1,
            "is_lead_source": false,
            "is_work_with_dp": false,
            "is_crm_template": false,
            "client_uuid": "36027253-34de-4ed8-a141-d26719e10030",
            "is_active_in_account": true,
            "pipeline_id": 123,
            "settings_template": [],
            "settings": null
        }
        """;

        // Act
        WidgetResponse? widget = JsonSerializer.Deserialize<WidgetResponse>(jsonWithNulls, _jsonOptions);

        // Assert
        widget.Should().NotBeNull();
        widget!.Id.Should().Be(88899900);
        widget.Code.Should().BeNull();
        widget.Version.Should().BeNull();
        widget.Rating.Should().Be(1);
        widget.IsLeadSource.Should().BeFalse();
        widget.IsDigitalPipelineAvailable.Should().BeFalse();
        widget.IsCrmTemplate.Should().BeFalse();
        widget.IsActiveInAccount.Should().BeTrue();
        widget.PipelineId.Should().Be(123);
        widget.SettingsTemplates.Should().BeEmpty();
        widget.WidgetSettings.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_Text_CustomField_Response()
    {
        var field = new CustomField
        {
            Id = 4439091,
            Name = "Пример текстового поля",
            Code = "TEXT_FIELD",
            Sort = 504,
            Type = CustomFieldType.Text,
            EntityType = EntityType.Leads,
            IsPredefined = false,
            IsApiOnly = false,
            GroupId = null,
            EnumValues = null,
            RequiredInStatuses =
            [
                new CustomFieldLeadStatus { StatusId = 41221, PipelineId = 3142 }
            ]
        };

        return Verify(field)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public Task Should_Verify_Select_CustomField_Response()
    {
        var field = new CustomField
        {
            Id = 3,
            Name = "Телефон",
            Code = "PHONE",
            Sort = 4,
            Type = CustomFieldType.Multitext,
            EntityType = EntityType.Contacts,
            IsPredefined = true,
            IsDeletable = false,
            GroupId = null,
            EnumValues =
            [
                new CustomFieldEnumValue { Id = 1, Value = "WORK", Sort = 2 },
                new CustomFieldEnumValue { Id = 5, Value = "MOB", Sort = 6, Code = "MOBILE" }
            ],
            RequiredInStatuses = []
        };

        return Verify(field)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public Task Should_Verify_ChainedList_CustomField_Response()
    {
        var field = new CustomField
        {
            Id = 1150985,
            Name = "Каталог товаров",
            Code = "PRODUCTS",
            Sort = 100,
            Type = CustomFieldType.ChainedList,
            EntityType = EntityType.Leads,
            IsApiOnly = false,
            ChainedListNodes =
            [
                new CustomFieldChainedListNode
                {
                    Title = "Категория",
                    CatalogId = 1001,
                    ParentCatalogId = 0
                },
                new CustomFieldChainedListNode
                {
                    Title = "Подкатегория",
                    CatalogId = 1007,
                    ParentCatalogId = 1001
                }
            ]
        };

        return Verify(field)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_CustomField_Response_From_Generated_Json()
    {
        // Arrange
        var originalField = new CustomField
        {
            Id = 12345,
            Name = "Дата встречи",
            Code = "MEETING_DATE",
            Sort = 10,
            Type = CustomFieldType.Date,
            EntityType = EntityType.Leads,
            IsPredefined = false,
            IsApiOnly = true,
            BirthdayReminderType = null,
            CurrencyCode = null,
            GroupId = "leads_29741591099841",
            EnumValues = null,
            RequiredInStatuses =
            [
                new CustomFieldLeadStatus { StatusId = 142, PipelineId = 1300 }
            ],
            HiddenInStatuses = null,
            ChainedListNodes = null,
            NestedValues = null
        };

        var json = JsonSerializer.Serialize(originalField, _jsonOptions);

        // Act
        CustomField? deserialized = JsonSerializer.Deserialize<CustomField>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalField.Id);
        deserialized.Name.Should().Be(originalField.Name);
        deserialized.Code.Should().Be(originalField.Code);
        deserialized.Sort.Should().Be(originalField.Sort);
        deserialized.Type.Should().Be(CustomFieldType.Date);
        deserialized.EntityType.Should().Be(EntityType.Leads);
        deserialized.IsApiOnly.Should().BeTrue();
        deserialized.GroupId.Should().Be("leads_29741591099841");

        deserialized.RequiredInStatuses.Should().NotBeNull().And.HaveCount(1);
        deserialized.RequiredInStatuses![0].StatusId.Should().Be(142);
        deserialized.RequiredInStatuses[0].PipelineId.Should().Be(1300);
    }

    [Fact]
    public void Should_Ignore_CustomField_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "Поле с типом checkbox",
            "code": "IS_ACTIVE",
            "sort": 200,
            "type": "checkbox",
            "entity_type": "contacts",
            "is_predefined": false,
            "is_api_only": false,
            "group_id": "default",
            "enums": null,
            "required_statuses": [],
            "future_field": "ignore_me",
            "extra_array": [1, 2, 3],
            "remind": null,
            "currency": null
        }
        """;

        // Act
        CustomField? field = JsonSerializer.Deserialize<CustomField>(jsonWithExtraFields, _jsonOptions);

        // Assert
        field.Should().NotBeNull();
        field!.Id.Should().Be(99900011);
        field.Name.Should().Be("Поле с типом checkbox");
        field.Code.Should().Be("IS_ACTIVE");
        field.Type.Should().Be(CustomFieldType.Checkbox);
        field.EntityType.Should().Be(EntityType.Contacts);
        field.IsPredefined.Should().BeFalse();
        field.IsApiOnly.Should().BeFalse();
        field.GroupId.Should().Be("default");
        field.EnumValues.Should().BeNull();
        field.RequiredInStatuses.Should().BeEmpty();
    }

    [Fact]
    public void Should_Handle_CustomField_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": "Поле без кода",
            "code": null,
            "sort": null,
            "type": "text",
            "entity_type": "leads",
            "is_computed": null,
            "is_predefined": null,
            "is_deletable": null,
            "is_visible": null,
            "is_required": null,
            "remind": null,
            "currency": null,
            "is_api_only": null,
            "group_id": null,
            "enums": null,
            "nested": null,
            "required_statuses": null,
            "hidden_statuses": null,
            "chained_lists": null
        }
        """;

        // Act
        CustomField? field = JsonSerializer.Deserialize<CustomField>(jsonWithNulls, _jsonOptions);

        // Assert
        field.Should().NotBeNull();
        field!.Id.Should().Be(88899900);
        field.Name.Should().Be("Поле без кода");
        field.Code.Should().BeNull();
        field.Sort.Should().BeNull();
        field.Type.Should().Be(CustomFieldType.Text);
        field.EntityType.Should().Be(EntityType.Leads);
        field.IsComputed.Should().BeNull();
        field.IsPredefined.Should().BeNull();
        field.IsDeletable.Should().BeNull();
        field.IsVisible.Should().BeNull();
        field.IsRequired.Should().BeNull();
        field.BirthdayReminderType.Should().BeNull();
        field.CurrencyCode.Should().BeNull();
        field.IsApiOnly.Should().BeNull();
        field.GroupId.Should().BeNull();
        field.EnumValues.Should().BeNull();
        field.NestedValues.Should().BeNull();
        field.RequiredInStatuses.Should().BeNull();
        field.HiddenInStatuses.Should().BeNull();
        field.ChainedListNodes.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_User_Response()
    {
        // Arrange
        var user = new User
        {
            Id = 123,
            FullName = "Иван Иванов",
            Email = "ivan@example.com",
            Language = "ru",
            Permissions = new UserPermissions
            {
                Leads = new UserEntityPermissions { View = UserPermissionType.All },
                Contacts = new UserEntityPermissions { View = UserPermissionType.Group },
                Companies = new UserEntityPermissions { View = UserPermissionType.Mine },
                Tasks = new UserEntityPermissions { View = UserPermissionType.Denied },
                IsMailAccessEnabled = true,
                IsCatalogAccessEnabled = false,
                StatusPermissions =
                [
                    new UserStatusPermissions
                    {
                        EntityType = EntityType.Leads,
                        PipelineId = 1001,
                        StatusId = 142,
                        EntityPermissions = new UserEntityPermissions { View = UserPermissionType.All }
                    }
                ],
                IsAdmin = false,
                IsFree = false,
                IsActive = true,
                GroupId = 625,
                RoleId = 101
            },
            Details = new UserDetails
            {
                Roles =
                [
                    new UserRole
                    {
                        Id = 101,
                        Name = "Менеджер",
                        SelfLink = new PaginationLinksResponse
                        {
                            Self = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/roles/101" }
                        }
                    }
                ],
                Groups =
                [
                    new UserGroup
                    {
                        Id = 625,
                        Name = "Отдел продаж"
                    }
                ]
            }
        };

        // Act & Assert
        return Verify(user)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_User_Response_From_Generated_Json()
    {
        // Arrange
        var originalUser = new User
        {
            Id = 504141,
            FullName = "Администратор",
            Email = "admin@example.com",
            Language = "en",
            Permissions = new UserPermissions
            {
                Leads = new UserEntityPermissions { View = UserPermissionType.All },
                Contacts = new UserEntityPermissions { View = UserPermissionType.All },
                Companies = new UserEntityPermissions { View = UserPermissionType.All },
                Tasks = new UserEntityPermissions { View = UserPermissionType.All },
                IsMailAccessEnabled = true,
                IsCatalogAccessEnabled = true,
                IsAdmin = true,
                IsFree = false,
                IsActive = true,
                GroupId = null,
                RoleId = null
            },
            Details = null
        };

        var json = JsonSerializer.Serialize(originalUser, _jsonOptions);

        // Act
        User? deserialized = JsonSerializer.Deserialize<User>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(originalUser.Id);
        deserialized.FullName.Should().Be(originalUser.FullName);
        deserialized.Email.Should().Be(originalUser.Email);
        deserialized.Language.Should().Be(originalUser.Language);

        deserialized.Permissions.Should().NotBeNull();
        deserialized.Permissions!.IsAdmin.Should().BeTrue();
        deserialized.Permissions.IsMailAccessEnabled.Should().BeTrue();
        deserialized.Permissions.Leads!.View.Should().Be(UserPermissionType.All);

        deserialized.Details.Should().BeNull();
    }

    [Fact]
    public void Should_Ignore_User_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "id": 99900011,
            "name": "Тестовый пользователь",
            "email": "test@example.com",
            "lang": "ru",
            "future_field": "ignore_me",
            "rights": {
                "leads": { "view": "A" },
                "contacts": { "view": "G" },
                "companies": { "view": "M" },
                "tasks": { "view": "D" },
                "mail_access": true,
                "catalog_access": false,
                "is_admin": false,
                "is_free": false,
                "is_active": true,
                "group_id": 100,
                "role_id": 200,
                "extra_rights_prop": "skip"
            },
            "_embedded": {
                "roles": [
                    {
                        "id": 200,
                        "name": "Роль",
                        "extra_role_prop": "skip",
                        "_links": {
                            "self": {
                                "href": "https://example.amocrm.ru/api/v4/roles/200"
                            }
                        }
                    }
                ],
                "groups": [
                    {
                        "id": 100,
                        "name": "Группа",
                        "extra_group_prop": "skip"
                    }
                ],
                "unknown_embedded": []
            }
        }
        """;

        // Act
        User? user = JsonSerializer.Deserialize<User>(jsonWithExtraFields, _jsonOptions);

        // Assert
        user.Should().NotBeNull();
        user!.Id.Should().Be(99900011);
        user.FullName.Should().Be("Тестовый пользователь");
        user.Email.Should().Be("test@example.com");
        user.Language.Should().Be("ru");

        user.Permissions.Should().NotBeNull();
        user.Permissions!.Leads!.View.Should().Be(UserPermissionType.All);
        user.Permissions.Contacts!.View.Should().Be(UserPermissionType.Group);
        user.Permissions.Companies!.View.Should().Be(UserPermissionType.Mine);
        user.Permissions.Tasks!.View.Should().Be(UserPermissionType.Denied);
        user.Permissions.IsAdmin.Should().BeFalse();
        user.Permissions.GroupId.Should().Be(100);
        user.Permissions.RoleId.Should().Be(200);

        user.Details.Should().NotBeNull();
        user.Details!.Roles.Should().HaveCount(1);
        user.Details.Roles![0].Id.Should().Be(200);
        user.Details.Roles[0].Name.Should().Be("Роль");

        user.Details.Groups.Should().HaveCount(1);
        user.Details.Groups![0].Id.Should().Be(100);
        user.Details.Groups[0].Name.Should().Be("Группа");
    }

    [Fact]
    public void Should_Handle_User_Response_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "id": 88899900,
            "name": null,
            "email": null,
            "lang": null,
            "rights": null,
            "_embedded": null
        }
        """;

        // Act
        User? user = JsonSerializer.Deserialize<User>(jsonWithNulls, _jsonOptions);

        // Assert
        user.Should().NotBeNull();
        user!.Id.Should().Be(88899900);
        user.FullName.Should().BeNull();
        user.Email.Should().BeNull();
        user.Language.Should().BeNull();
        user.Permissions.Should().BeNull();
        user.Details.Should().BeNull();
    }

    [Fact]
    public Task Should_Verify_AuthorizationTokens_Response()
    {
        // Arrange
        var tokens = new AuthorizationTokens
        {
            ExpiresIn = 86400,
            AccessToken = "access_abc123xyz",
            RefreshToken = "refresh_def456uvw"
        };

        // Act & Assert
        return Verify(tokens)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_AuthorizationTokens_Response_From_Generated_Json()
    {
        // Arrange
        var originalTokens = new AuthorizationTokens
        {
            ExpiresIn = 3600,
            AccessToken = "access_abc123xyz",
            RefreshToken = "refresh_def456uvw"
        };

        var json = JsonSerializer.Serialize(originalTokens, _jsonOptions);

        // Act
        AuthorizationTokens? deserialized = JsonSerializer.Deserialize<AuthorizationTokens>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.ExpiresIn.Should().Be(originalTokens.ExpiresIn);
        deserialized.AccessToken.Should().Be(originalTokens.AccessToken);
        deserialized.RefreshToken.Should().Be(originalTokens.RefreshToken);
    }

    [Fact]
    public void Should_Ignore_AuthorizationTokens_Response_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "expires_in": 7200,
            "access_token": "access_token_value",
            "refresh_token": "refresh_token_value",
            "token_type": "Bearer",
            "scope": "read write",
            "future_field": "ignore_me",
            "extra_array": [1, 2, 3]
        }
        """;

        // Act
        AuthorizationTokens? tokens = JsonSerializer.Deserialize<AuthorizationTokens>(jsonWithExtraFields, _jsonOptions);

        // Assert
        tokens.Should().NotBeNull();
        tokens!.ExpiresIn.Should().Be(7200);
        tokens.AccessToken.Should().Be("access_token_value");
        tokens.RefreshToken.Should().Be("refresh_token_value");
    }

    [Fact]
    public Task Should_Verify_BaseResponse_With_ValidationError()
    {
        // Arrange
        var response = new BaseResponse
        {
            ErrorTitle = "Bad Request",
            ErrorDetails = "Request validation failed",
            ValidationErrors = new ValidationErrors
            {
                RequestId = "0",
                Errors =
                [
                    new ValidationError
                    {
                        Code = "NotSupportedChoice",
                        Path = "custom_fields_values.0.field_id",
                        Detail = "The value you selected is not a valid choice."
                    }
                ]
            }
        };

        // Act & Assert
        return Verify(response)
            .UseDirectory("Fixtures/Snapshots");
    }

    [Fact]
    public void Should_Deserialize_BaseResponse_From_Generated_Json()
    {
        // Arrange
        var originalResponse = new BaseResponse
        {
            ErrorTitle = "Bad Request",
            ErrorDetails = "Request validation failed",
            ValidationErrors = new ValidationErrors
            {
                RequestId = "0",
                Errors =
                [
                    new ValidationError
                    {
                        Code = "NotSupportedChoice",
                        Path = "custom_fields_values.0.field_id",
                        Detail = "The value you selected is not a valid choice."
                    }
                ]
            }
        };

        var json = JsonSerializer.Serialize(originalResponse, _jsonOptions);

        // Act
        BaseResponse? deserialized = JsonSerializer.Deserialize<BaseResponse>(json, _jsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.ErrorTitle.Should().Be("Bad Request");
        deserialized.ErrorDetails.Should().Be("Request validation failed");
        deserialized.ValidationErrors.Should().NotBeNull();
        deserialized.ValidationErrors!.Errors.Should().NotBeNull().And.HaveCount(1);
        deserialized.ValidationErrors.RequestId.Should().Be("0");

        ValidationError validationError = originalResponse.ValidationErrors.Errors![0];
        validationError.Code.Should().Be("NotSupportedChoice");
        validationError.Path.Should().Be("custom_fields_values.0.field_id");
        validationError.Detail.Should().Be("The value you selected is not a valid choice.");
    }

    [Fact]
    public void Should_Ignore_BaseResponse_Unknown_Json_Properties()
    {
        // Arrange
        var jsonWithExtraFields = """
        {
            "title": "Bad Request",
            "detail": "Request validation failed",
            "type": "https://httpstatus.es/400",
            "status": 400,
            "extra_top_level_field": "ignore_me",
            "validation-errors": {
                "request_id": "0",
                "errors": [
                    {
                        "code": "NotSupportedChoice",
                        "path": "custom_fields_values.0.field_id",
                        "detail": "The value you selected is not a valid choice.",
                        "extra_error_prop": "skip"
                    }
                ],
                "extra_validation_prop": "skip"
            }
        }
        """;

        // Act
        BaseResponse? response = JsonSerializer.Deserialize<BaseResponse>(jsonWithExtraFields, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.ErrorTitle.Should().Be("Bad Request");
        response.ErrorDetails.Should().Be("Request validation failed");

        response.ValidationErrors.Should().NotBeNull();
        response.ValidationErrors!.RequestId.Should().Be("0");
        response.ValidationErrors!.Errors.Should().NotBeNull().And.HaveCount(1);

        ValidationError validationError = response.ValidationErrors.Errors![0];

        validationError.Code.Should().Be("NotSupportedChoice");
        validationError.Path.Should().Be("custom_fields_values.0.field_id");
        validationError.Detail.Should().Be("The value you selected is not a valid choice.");
    }

    [Fact]
    public void Should_Handle_BaseResponse_Json_With_Null_Values()
    {
        // Arrange
        var jsonWithNulls = """
        {
            "title": null,
            "detail": null,
            "validation-errors": null
        }
        """;

        // Act
        BaseResponse? response = JsonSerializer.Deserialize<BaseResponse>(jsonWithNulls, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.ErrorTitle.Should().BeNull();
        response.ErrorDetails.Should().BeNull();
        response.ValidationErrors.Should().BeNull();
    }
}

CREATE TABLE "ReadingByZip"(  
    "Id" int NOT NULL PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "LastUpdatedUtc" timestamp with time zone,
    "LastLookupUtc" timestamp with time zone,
    "Zip" VARCHAR(5),
    "TemperatureInCelcius" NUMERIC(4, 1)
);

--drop table "ReadingByZip"b

INSERT INTO "ReadingByZip"
("LastUpdatedUtc", "LastLookupUtc", "Zip", "TemperatureInCelcius")
VALUES
(now() at time zone ('utc'), now() at time zone ('utc'), '54703', 0)

select * from "ReadingByZip"
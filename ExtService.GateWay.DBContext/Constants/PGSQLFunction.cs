namespace ExtService.GateWay.DBContext.Constants
{
    public static class PGSQLFunction
    {
        public const string LogTransactionsTriggerFunctionName = "log_transaction";

        public const string LogTransactionsTriggerFunctionCreateCommand = @$"
            CREATE OR REPLACE FUNCTION {PGSQLConsts.PublicSchema}.{LogTransactionsTriggerFunctionName}()
            RETURNS TRIGGER AS $$
            DECLARE
                _transactionLogId uuid;
                _tableName text;
                _operation text;
                _oldValue text;
                _newValue text;
                _createdAt timestamp;
                _dbUser text;
            BEGIN
                _transactionLogId := uuid_generate_v4();
                _tableName := TG_TABLE_NAME;
                _operation := TG_OP;
                _oldValue := row_to_json(OLD)::text;
                _newValue := row_to_json(NEW)::text;
                _createdAt := now();
                _dbUser := current_user;
                INSERT INTO public.""TransactionLog"" (""TransactionLogId"", ""TableName"", ""Operation"", ""OldValue"", ""NewValue"", ""CreatedAt"", ""DBUser"")
                VALUES (_transactionLogId, _tableName, _operation, _oldValue, _newValue, _createdAt, _dbUser);
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;";

        public const string LogTransactionsTriggerFunctionDropCommand = @$"
            DROP FUNCTION IF EXISTS {PGSQLConsts.PublicSchema}.{LogTransactionsTriggerFunctionName}();";

        public const string EncryptPluginParameterValueTriggerFunctionName = "encrypt_plugin_parameter_value";

        public const string EncryptPluginParameterValueTriggerFunctionCreateCommand = @$"
            CREATE OR REPLACE FUNCTION {PGSQLConsts.PublicSchema}.{EncryptPluginParameterValueTriggerFunctionName}()
            RETURNS TRIGGER AS $$
            BEGIN
                IF ((TG_OP = 'INSERT' OR TG_OP = 'UPDATE') AND NEW.""ParameterValue"" IS NOT NULL) THEN
                    NEW.""ParameterValue"" := encode(pgp_sym_encrypt(NEW.""ParameterValue"", '40iJwUXK2McccbSkxeksPdp2v6vjZmSY'), 'base64');
                END IF;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;";

        public const string EncryptPluginParameterValueTriggerFunctionDropCommand = @$"
            DROP FUNCTION IF EXISTS {PGSQLConsts.PublicSchema}.{EncryptPluginParameterValueTriggerFunctionName}();";

        public const string DecryptPluginParameterValueFunctionName = "decrypt_plugin_parameter_value";

        public const string DecryptPluginParameterValueFunctionCreateCommand = @$"
            CREATE OR REPLACE FUNCTION {PGSQLConsts.PublicSchema}.{DecryptPluginParameterValueFunctionName}(encrypted_text TEXT)
            RETURNS TEXT AS $$
            BEGIN
                RETURN pgp_sym_decrypt(decode(NEW.""ParameterValue"",'base64'), '{TriggerConstants.EncriptionKey}');
            END;
            $$ LANGUAGE plpgsql;";

        public const string DecryptPluginParameterValueFunctionDropCommand = @$"
            DROP FUNCTION IF EXISTS {PGSQLConsts.PublicSchema}.{DecryptPluginParameterValueFunctionName}();";
    }
}

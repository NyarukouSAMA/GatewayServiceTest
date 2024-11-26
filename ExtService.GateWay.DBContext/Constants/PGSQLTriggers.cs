namespace ExtService.GateWay.DBContext.Constants
{
    public static class PGSQLTriggers
    {
        public const string LogTransactionsTriggerForIdentificationTableName = "trg_identification_logging";

        public const string LogTransactionsTriggerForIdentificationTableCreateCommand = $@"
            CREATE TRIGGER {LogTransactionsTriggerForIdentificationTableName}
            AFTER INSERT OR UPDATE OR DELETE
            ON {PGSQLConsts.PublicSchema}.""Identification""
            FOR EACH ROW
            EXECUTE FUNCTION {PGSQLFunction.LogTransactionsTriggerFunctionName}();";

        public const string LogTransactionsTriggerForIdentificationTableDropCommand = $@"
            DROP TRIGGER IF EXISTS {LogTransactionsTriggerForIdentificationTableName} ON {PGSQLConsts.PublicSchema}.""Identification"";";

        public const string EncryptPluginParametersTriggerForPluginParameterTableName = "trg_plugin_parameters_encryption";

        public const string EncryptPluginParametersTriggerForPluginParameterTableColumns = "ParameterValue";

        public const string EncryptPluginParametersTriggerForPluginParameterTableCreateCommand = $@"
            CREATE TRIGGER {EncryptPluginParametersTriggerForPluginParameterTableName}
            BEFORE INSERT OR UPDATE
            OF {EncryptPluginParametersTriggerForPluginParameterTableColumns}
            ON {PGSQLConsts.PublicSchema}.""PluginParameter""
            FOR EACH ROW
            EXECUTE FUNCTION {PGSQLFunction.EncryptPluginParameterValueTriggerFunctionName}();";

        public const string EncryptPluginParametersTriggerForPluginParameterTableDropCommand = $@"
            DROP TRIGGER IF EXISTS {EncryptPluginParametersTriggerForPluginParameterTableName} ON {PGSQLConsts.PublicSchema}.""PluginParameter"";";
    }
}

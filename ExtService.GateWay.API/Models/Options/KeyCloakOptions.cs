namespace ExtService.GateWay.API.Models.Options
{
    public class KeyCloakOptions
    {
        public static string KeyCloakConfigSection = "KeyCloak";
        public string KeyCloakDomain { get; set; }
        public string KeyCloakRealm { get; set; }
        public string KeyCloakRealmAuthority => $"{KeyCloakDomain}/realms/{KeyCloakRealm}";
        public string KeyCloakRealmMetadataEndpoint => $"{KeyCloakRealmAuthority}/.well-known/openid-configuration";
    }
}

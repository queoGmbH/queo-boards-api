using System;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.Builders {
    public static class AttachmentDownloadTokenizer {

        public const string SECRET = "arfhzAswiTwbtrf8bz9owr3btf68wbz6erfv789nz624897wrfn";
        
        public static string GetToken(AttachmentDownload attachmentDownload) {
            Require.NotNull(attachmentDownload, "attachmentDownload");

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            string token = encoder.Encode(attachmentDownload, SECRET);
            return token;
        }

        public static AttachmentDownload ParseFromToken(string token) {
            try {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                AttachmentDownload attachmentDownload = decoder.DecodeToObject<AttachmentDownload>(token, SECRET, true);
                return attachmentDownload;
            } catch (Exception ex) {
                return null;
            }
        }

    }
}
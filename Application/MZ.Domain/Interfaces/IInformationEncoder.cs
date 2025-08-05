namespace MZ.Domain.Interfaces
{
    /// <summary>
    /// Sha256 Information Encoder : SHA256 해시 기반 정보 인코더
    /// - 텍스트(비밀번호 등)를 SHA256 해시로 변환하여 암호화/저장 용도
    /// - 인증, 보안 검증을 위함
    /// </summary>
    public interface IInformationEncoder
    {
        /// <summary>
        /// 입력 문자열을 SHA256 해시 후 Base64 문자열로 변환하여 반환
        /// </summary>
        /// <param name="value">string : 암호화할 원본 문자열(예: 비밀번호)</param>
        /// <returns>string : Base64로 인코딩된 SHA256 해시 문자열</returns>
        public string Hash(string value);

        /// <summary>
        /// 원본 문자열을 해시하여 기존 해시값과 비교
        /// </summary>
        /// <param name="hashed">string : 이미 저장되어 있는 해시값</param>
        /// <param name="raw">string : 사용자가 입력한 원본 값</param>
        /// <returns>bool : 일치 여부(true: 동일, false: 불일치)</returns>
        public bool Verify(string hashed, string raw);
    }
}

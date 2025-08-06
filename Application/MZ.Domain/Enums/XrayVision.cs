using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    /// <summary>
    /// Xray/이미지 분석 시스템에서 사용하는 색상(컬러맵) 분류 Enum
    /// 
    /// - 이미지 표시, 분석 결과 컬러맵, 필터 적용 등에 활용
    /// - Name: 시스템 내부/색상 코드
    /// - Description: UI 노출용/분류 명칭
    /// </summary>
    public enum ColorRole
    {
        /// <summary>
        /// 그레이스케일(흑백) 모드  
        /// </summary>
        [Display(Name = "Gray", Description = "Gray")]
        Gray,

        /// <summary>
        /// 컬러(ColorMap) 모드 
        /// </summary>
        [Display(Name = "Color", Description = "Color")]
        Color,
        /// <summary>
        /// 유기물(Organic) 강조 컬러  
        /// - 식품, 플라스틱 등 감지용
        /// </summary>
        [Display(Name = "Organic", Description = "Organic")]
        Organic,
        /// <summary>
        /// 무기물(Inorganic) 강조 컬러  
        /// - 흙, 석재, 시멘트 등
        /// </summary>
        [Display(Name = "Inorganic", Description = "Inorganic")]
        Inorganic,
        /// <summary>
        /// 금속(Metal) 강조 컬러  
        /// - 철, 알루미늄, 구리 등 금속 감지
        /// </summary>
        [Display(Name = "Metal", Description = "Metal")]
        Metal,
    }
}

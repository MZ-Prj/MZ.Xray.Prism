using MZ.Domain.Interfaces;
using MZ.Resource;
using Prism.Mvvm;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace MZ.Model
{
    public class IconButtonModel : BindableBase, IUserButton, IDisposable
    {
        /// <summary>
        /// 버튼 고유 Id (DB, 세팅 저장 등 활용)
        /// </summary>
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        /// <summary>
        /// 추가 식별값 (분류/유형/역할 등 자유 활용, 예: Enum/Key/Guid)
        /// </summary>
        private object _uid;
        public object UId { get => _uid; set => SetProperty(ref _uid, value); }

        /// <summary>
        /// 버튼 표시 여부 (IsVisible)
        /// </summary>
        private bool _isVisibility = true;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }

        /// <summary>
        /// 아이콘 종류 (PackIconMaterial 문자열)
        /// </summary>
        private string _iconKind;
        public string IconKind { get => _iconKind; set => SetProperty(ref _iconKind, value); }

        /// <summary>
        /// 버튼 이름/식별용 키 
        /// </summary>
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        /// <summary>
        /// 버튼 이름/식별용 키 
        /// </summary>
        public string _tooltip;
        public string Tooltip { get => _tooltip; set => SetProperty(ref _tooltip, value); }

        /// <summary>
        /// 버튼 클릭시 실행될 ICommand
        /// </summary>
        private ICommand _command;
        public ICommand Command { get => _command; set => SetProperty(ref _command, value); }

        /// <summary>
        /// 아이콘/버튼 색상(Brush)
        /// </summary>
        private Brush _colorBrush;
        public Brush ColorBrush { get => _colorBrush; set => SetProperty(ref _colorBrush, value); }

        /// <summary>
        /// 아이콘버튼 객체 생성자  
        /// - 필수: 아이콘 종류, 커맨드  
        /// - 옵션: 컬러, 표시여부, UId, 이름, 툴팁, Id 등
        /// </summary>
        /// <param name="iconKind">PackIcon 종류 등(문자열)</param>
        /// <param name="command">버튼 클릭시 실행 ICommand</param>
        /// <param name="colorBrush">아이콘 색상</param>
        /// <param name="isVisibility">표시 여부</param>
        /// <param name="uid">추가 식별값</param>
        /// <param name="name">버튼명</param>
        /// <param name="tooltipKey">툴팁 Key</param>
        /// <param name="id">Id</param>
        public IconButtonModel(string iconKind, ICommand command, Brush colorBrush = null, bool isVisibility = true, object uid = null, string name = null, string tooltipKey = "", int id = 0)
        {
            Id = id;
            IsVisibility = isVisibility;
            Command = command;
            IconKind = iconKind;
            ColorBrush = colorBrush;
            UId = uid ?? iconKind;
            Name = name;
            Tooltip = tooltipKey;

            LanguageService.LanguageChanged += LanguageChanged;
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            UpdateTooltip();
        }

        public void UpdateTooltip()
        {
            RaisePropertyChanged(nameof(Tooltip));
        }

        public void Dispose()
        {
            LanguageService.LanguageChanged -= LanguageChanged;
        }

    }
}

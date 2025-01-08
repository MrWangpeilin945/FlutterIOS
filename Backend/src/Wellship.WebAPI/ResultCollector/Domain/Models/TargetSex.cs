using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 対象性別
/// </summary>
public class TargetSex
{
    /// <summary>
    /// 対象性別
    /// </summary>
    public TargetSexType TargetSexType { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public TargetSex(TargetSexType targetSexType)
    {
        TargetSexType = targetSexType;
    }

    /// <summary>
    /// 対象性別か判断する
    /// </summary>
    public bool IsMatch(Sex sex)
    {
        return (TargetSexType, sex) switch
        {
            (TargetSexType.両方, _) => true,
            (TargetSexType.男, Sex.男) => true,
            (TargetSexType.女, Sex.女) => true,
            _ => false
        };
    }
}

namespace SmartShop.DTO;

public enum SearchOperator
{
    Contains, Equal, NotEqual, GreaterThan, GreaterOrEqual, LessThan, LessOrEqual
}

public enum SearchFieldType { Text, Number }

public sealed record SearchField(string Key, string Label, SearchFieldType Type = SearchFieldType.Text);
public sealed record SearchCondition(string Field, SearchOperator Operator, string Value, bool Negate = false);

// Các điều kiện trong cùng nhóm nối bằng AND; các nhóm nối với nhau bằng OR.
public sealed record SearchGroup(IReadOnlyList<SearchCondition> Conditions);
public sealed record CatalogFilter(IReadOnlyList<SearchGroup> Groups);

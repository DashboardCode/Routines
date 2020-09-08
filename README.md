# Routines
*Fixing abstractions*

Functional framework to compose functions parametrized by object navigation expression (such functions as "clone", "compare", "serialize" etc.). Provides additional abstractions for function's composition.


It is based on DSL Include idea:

 
Serialization to JSON:

```
var formatter = JsonManager.ComposeFormatter<Point>(   // or ComposeEnumerableFormatter
   chain=>chain   
      // DSL Includes
      .Include(e=>e.X)  // leaf
      .Include(e=>e.Y)  // leaf 
           .TnenInclude(e=>e.NextPoint) // navigation node
              .TnenIncluding(e=>e.X) // node
              .TnenIncluding(e=>e.Y) // node
); // save it to reuse
var json = formatter2(new Point{X=1,Y=-1});
```

More options:

```
var formatter = JsonManager.ComposeEnumerableFormatter(include
                    , rootHandleNullArray: false // root null or empty
                    , handleNullProperty: false  // include properties that are null
                    , handleNullArrayProperty: false  // include array properties that are null
                    , useToString: true // if there are not default formatter 
                    , useToString: false,
                    , dateTimeFormat: "YYYYMMDD", 
                    , floatingPointFormat: "M4"
            );
```

More options **SubTree**:

```
var formatter2 = JsonManager.ComposeFormatter(
                include,
                rules => rules
                    .SubTree(
                          chain => chain.Include(e => e.Text2),
                          stringAsJsonLiteral: true  // this is json string, include 'as is'
                    )
            );
```


More options **AddRule** 

```
var formatter = JsonManager.ComposeFormatter(
                include,
                rules => rules
                    .AddRule<string[]>(GetStringArrayFormatter) // add formatter for special type
                    .AddRule<int[]>((sb, l) => GetStringIntFormatter(sb, l))
                    .AddRule<IEnumerable<Guid>>(GetStringGuidFormatter) 
                    // add formatter for special type but only for subTree
                    .SubTree(
                        chain  => chain.Include(e => e.Test),
                        subRules => subRules.AddRule<int[]>(serializer: GetSumFormatter, propertySerializationName: "Sum"),
                        dateTimeFormat: null, 
                        floatingPointFormat: null
                    ));
```

## Equals

```
Include<User> include = chain=>chain.Include(e=>e.UserId).IncludeAll(e=>e.Groups).ThenInclude(e=>e.GroupId)

bool b1 = ObjectExtensions.Equals(user1, user2, include);
bool b2 = ObjectExtensions.EqualsAll(userList1, userList2, include);
```

## Clone
```cs
Include<User> include = chain=>chain.Include(e=>e.UserId).IncludeAll(e=>e.Groups).ThenInclude(e=>e.GroupId)

var newUser = ObjectExtensions.Clone(user1, include, leafRule1);
var newUserList = ObjectExtensions.CloneAll(userList1, leafRule1);
```

## Copy

```cs
Include<User> include = chain=>chain.IncludeAll(e=>e.Groups);

ObjectExtensions.Copy(user1, user2, include, supportedLeafsRule);  
ObjectExtensions.CopyAll(userList1, userList2, include, supportedLeafsRule);
```

## DSL Includes Internal Structures

```
ChainNode root = include.CreateChainNode();
```
There root contains .Children - `Dictionary<srting, ChainMemberNode>` 

`ChainMemberNode` type - additionally contains `.Parent` - ref to parent.

You can create `includes` dinamically:

```cs
var root = new ChainNode(typeof(Point));
var child = new ChainPropertyNode(
         typeof(int),
         expression: typeof(Point).CreatePropertyLambda("X"),
         memberName:"X", isEnumerable:false, parent:root
);
root.Children.Add("X", child);
// or there is number of extension methods e.g.: var child = root.AddChild("X");

Include<Point> include = ChainNodeExtensions.ComposeInclude<Point>(root);
```


## DSL Includes Meta operations

Add leafs by rule
```
Func<ChainNode, MemberName> leafRule = ... //
inlcude.AppendLeafs(leafRule ?? LeafRuleManager.DefaultEfCore);
```


Compare:
```cs
var b1 = InlcudeExtensions.IsEqualTo(include1, include2);
var b2 = InlcudeExtensions.IsSubTreeOf(include1, include2);
var b3 = InlcudeExtensions.IsSuperTreeOf(include1, include2);
```
</spoiler>

Clone:
```cs
var include2 = InlcudeExtensions.Clone(include1);
```

Merge:
```cs
var include3 = InlcudeExtensions.Equals(include1, include2);
```

Get XPATH to all leafs:
```cs
IReadOnlyCollection<string> paths1 = InlcudeExtensions.ListLeafXPaths(include);
```

 


[![Coverage Status](https://s3.amazonaws.com/assets.coveralls.io/badges/coveralls_78.svg)](https://coveralls.io/github/rpokrovskij/Vse?branch=)

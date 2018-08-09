# Routines
*Fixing abstractions*

Functional framework to compose functions parametrized by object navigation expression (such functions as "clone", "compare", "serialize" etc.) and 
provide some popular but missed in standard framework abstractions.


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

To be continue

[![Coverage Status](https://s3.amazonaws.com/assets.coveralls.io/badges/coveralls_78.svg)](https://coveralls.io/github/rpokrovskij/Vse?branch=)

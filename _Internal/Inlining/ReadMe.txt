Inlined tokens serve two purposes:
    1. To construct a version of the token that is independent of 
       other referenced tokens.  This is primarily to ensure that the
       token's state machine is not dependent upon other machines,
       and that the resulted token sub-set calculations can be
       reduced by eliminating tokens which were referred to by other
       tokens only.
       
    2. To provide a version of the tokens that exposes its state
       building facilities without cluttering the main token 
       definition used during the parse of a grammar description 
       file.
# Chikatto (WIP)
## osu! server (bancho+web+avatar) implementation

based on ripple database scheme so in future should be 
compatible with all ripple stack servers

PP Calculation through osu!Lazer (!)

- [ ] Bancho
    - [X] Logging in
    - [X] Chat
    - [X] Multiplayer rooms
    - [X] Spectating
    - [ ] Tournament stuff
    - [ ] Bot
        - [X] Admin commands
            - [X] (un)Ban
            - [X] (un)Restrict
            - [X] Give supporter
        - [X] Moderator commands
            - [X] (un)Silence
        - [ ] Beatmap nominator commands
            - [ ] Rank beatmap
            - [ ] Love beatmap
            - [ ] Unrank beatmap
        - [X] Owner commands
            - [X] Manage staff
                - [X] Add admin
                - [X] Add mod
                - [X] Add BN
                - [X] Add owner
        - [ ] Multi commands
            - [ ] make \<name> 
            - [ ] addref \<username> [\<username>] 
            - [ ] removeref \<username> [\<username>]
            - [ ] listrefs
            - [ ] size \<size>
            - [ ] close
            - [ ] map \<mapid>
            - [ ] mods \<mod> [\<mod>] [\<mod>]
            - [ ] start
              - [ ] start \<time> 
            - [ ] timer [\<time>]
            - [ ] aborttimer
            - [ ] invite \<username>
            - [ ] lock
            - [ ] unlock
            - [ ] set \<teammode> [\<scoremode>] [\<size>]
            - [ ] move \<username> \<slot>
            - [ ] host \<username>
        - [ ] Other commands (wip)
- [ ] Web
    - [X] osu!direct
    - [ ] Screenshot uploading
    - [X] Screenshot viewing
    - [X] Score submission
    - [ ] Map leaderboards
        - [ ] Global
        - [ ] Friends
        - [ ] Country
    - [ ] Other stuff (todo list wip lol)
- [X] Avatar server
- [X] In-client registration
- [ ] Api
    - [ ] RippleApi
    - [ ] PeppyApi
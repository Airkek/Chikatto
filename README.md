# Chikatto (WIP)
## osu! server (bancho+web+avatar) implementation

based on ripple database scheme so in future should be 
compatible with all ripple stack servers

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
            - [ ] Give supporter
        - [X] Moderator commands
            - [X] (un)Silence
        - [ ] Beatmap nominator commands
            - [ ] Rank beatmap
            - [ ] Love beatmap
            - [ ] Unrank beatmap
        - [ ] Owner commands
            - [ ] Manage staff
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
    - [ ] Score submission
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
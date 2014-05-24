/*
Module Dependencies 
*/
var express = require('express'),
    http = require('http'),
    path = require('path'),
    mongoose = require('mongoose'),
    hash = require('./pass').hash;

var app = express();

/*
Database and Models
*/
mongoose.connect("mongodb://jp:admin@ds030817.mongolab.com:30817/statusupdate");
var UserSchema = new mongoose.Schema({
    username: String,
    password: String,
    salt: String,
    hash: String,
	services: Array
});
var WebSitesSchema = new mongoose.Schema({
    site: String,
    twitter: String
});

var User = mongoose.model('users', UserSchema);
var Sites = mongoose.model('websites', WebSitesSchema);
/*
Middlewares and configurations 
*/
app.configure(function () {
    app.use(express.bodyParser());
    app.use(express.cookieParser('StatusUpdade'));
    app.use(express.session());
    app.use(express.static(path.join(__dirname, 'public')));
    app.set('views', __dirname + '/views');
    app.set('view engine', 'jade');
});

app.use(function (req, res, next) {
    var err = req.session.error,
        msg = req.session.success;
    delete req.session.error;
    delete req.session.success;
    res.locals.message = '';
    if (err) res.locals.message =err ;
    if (msg) res.locals.message = msg ;
    next();
});
/*
Helper Functions
*/
function authenticate(name, pass, fn) {
    if (!module.parent) console.log('authenticating %s:%s', name, pass);

    User.findOne({
        username: name
    },

    function (err, user) {
        if (user) {
            if (err) return fn(new Error('cannot find user'));
            hash(pass, user.salt, function (err, hash) {
                if (err) return fn(err);
                if (hash == user.hash) return fn(null, user);
                fn(new Error('invalid password'));
            });
        } else {
            return fn(new Error('cannot find user'));
        }
    });

}

function requiredAuthentication(req, res, next) {
    if (req.session.user) {
        next();
    } else {
        req.session.error = 'Access denied!';
        res.redirect('/login');
    }
}

function userExist(req, res, next) {
    User.count({
        username: req.body.username
    }, function (err, count) {
        if (count === 0) {
            next();
        } else {
            req.session.error = "User Exist"
			res.send('user exists');
        }
    });
}

/*
Routes
*/
app.get("/", function (req, res) {
    res.render("index");
});

app.post("/signup", userExist, function (req, res) {
    var password = req.body.password;
    var username = req.body.username;

    hash(password, function (err, salt, hash) {
        if (err) throw err;
        var user = new User({
            username: username,
            salt: salt,
            hash: hash,
        }).save(function (err, newUser) {
            if (err) throw err;
            authenticate(newUser.username, password, function(err, user){
                if(user){
                    req.session.regenerate(function(){
                        req.session.user = user;
                        req.session.success = 'Authenticated as ' + user.username ;
                        res.send('Authenticated as ' + user.username);
                    });
                }
            });
        });
    });
});


app.post("/login", function (req, res) {
    authenticate(req.body.username, req.body.password, function (err, user) {
        if (user) {
            req.session.regenerate(function () {
                req.session.user = user;
				res.send('authenticated as ' + user.username);
            });
        } else {
			res.send('login failed');
        }
    });
});

app.post('/apps', function (req, res) {
	var user = req.body.username;
	
	User.findOne({
        username: user
    },
    function (err, user) {
        if (user) {
            if (err) return fn(new Error('cannot find user'));
			req.session.destroy(function () {
				res.send(user.services);
			});
        } else {
            return fn(new Error('cannot find user'));
        }
    });
});

app.post('/default', function (req, res) {
	var wsite = req.body.site;
	Sites.findOne({
        site: wsite
    },
    function (err, wsite) {
        if (wsite) {
            if (err) return fn(new Error('cannot find wsite'));
			req.session.destroy(function () {
				res.send(wsite.twitter);
			});
        } else {
            return fn(new Error('cannot find wsite'));
        }
    });
});

app.post('/addsite',function (req, res) {
	var s = req.body.site;
	var t = req.body.twitter;
	var ad = new Sites({
			site : s,
			twitter: t});
	
	ad.save(function (err, ad) {
	  if (err) return console.error(err);
	  res.send("Added");
	});
	res.send("Added");
}); 

app.get('/allsites',function (req, res) {
	Sites.find(function (err, s) {
	  if (err) return console.error(err);
	  res.send(s);
	})
}); 

app.post('/subscribe', function (req, res) {
	var user = req.body.username;
	var ser = req.body.service;
	User.findOne({
        username: user
    },
    function (err, user) {
	console.log(arguments)
        if (user) {
            if (err) return fn(new Error('cannot find user'));
			req.session.destroy(function () {
				User.findOneAndUpdate(
					{username: user.username},
					{$push: {services: ser}},
					{safe: true, upsert: true},
					function(err, model) {
						console.log(err);
					}
				);
				res.send("Added");
			});
        } else {
            return fn(new Error('cannot find user'));
        }
    });
});

app.post('/remove', function (req, res) {
	var user = req.body.username;
	var ser = req.body.service;
	User.findOne({
        username: user
    },
    function (err, user) {
	console.log(arguments)
        if (user) {
            if (err) return fn(new Error('cannot find user'));
			req.session.destroy(function () {
				User.findOneAndUpdate(
					{username: user.username},
					{$pop: {services: ser}},
					{safe: true, upsert: true},
					function(err, model) {
						console.log(err);
					}
				);
				res.send("removed");
			});
        } else {
            return fn(new Error('cannot find user'));
        }
    });
});

app.post('/logout', function (req, res) {
    req.session.destroy(function () {
        res.send('terminated');
    });
});

app.set('port', (process.env.PORT || 5000));

app.listen(app.get('port'), function() {
  console.log("Node app is running at localhost:" + app.get('port'))
})

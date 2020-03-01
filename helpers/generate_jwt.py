#!/usr/bin/env python3

import jwt
import time

def main():
    encoded_jwt = jwt.encode(
        {
            'iss': 'meualelo.alelo.com.br',
            'sub': 'meualelo',
            'exp': int(round(time.time() * 10)),
            'fnp': 'fe2ae307aceb5898dd89799241a55676',
            'src': "WEB"
        }, '<hb(yk%YK8s{tw6T', algorithm='HS256')

    print(encoded_jwt)

if __name__ == '__main__':
    main()
